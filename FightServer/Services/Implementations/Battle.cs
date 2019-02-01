using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FightServer.HttpClients;
using FightServer.Models;
using FightServer.Services.Interfaces;
using FightServer.Settings;
using GameLogic.Implementations.Game;
using GameLogic.Interfaces.Public;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FightServer.Services.Implementations
{
	internal sealed class Battle
	{
		private readonly IDockerService dockerService;
		private readonly ICollection<string> dockerImages;
		private readonly Game game;
		private readonly ILogger<Battle> logger;
		private readonly IStorageClient storageClient;
		private readonly TimeSpan warmDelay;
		private readonly TimeSpan answerDelay;
		
		private IDictionary<string, string> dockerContainerIds;
		public readonly BattleInfo battleInfo;

		private async Task StartContainers()
		{
			this.dockerContainerIds = new Dictionary<string, string>(this.dockerImages.Count);
			foreach (var dockerImage in this.dockerImages)
			{
				try
				{
					var containerId = await this.dockerService.CreateAndStartContainer(dockerImage);
					this.dockerContainerIds.Add(containerId, dockerImage);
				}
				catch (Exception ex)
				{
					this.logger.LogCritical(ex, "Невозможно создать контейнер");
					throw;
				}
			}
		}

		public async Task<BattleInfo> Start(CancellationToken cancellationToken)
		{
			await this.storageClient.StartNewBattle(this.battleInfo);
			
			await this.StartContainers();

#pragma warning disable 4014
			this.StartMoves(cancellationToken);
#pragma warning restore 4014

			return this.battleInfo;
		}

		public async Task StartMoves(CancellationToken cancellationToken)
		{
			await Task.Delay(this.warmDelay, cancellationToken);
			uint i = 0;
			IReadOnlyDictionary<string, string> debugOutput = null;
			while (!cancellationToken.IsCancellationRequested && !this.game.IsEnded())
			{
				await this.storageClient.AddFrame(this.battleInfo.BattleId, this.BuildFrame(i++, debugOutput));
				
				debugOutput = await this.Tick();
			}
			
			await this.storageClient.AddFrame(this.battleInfo.BattleId, this.BuildFrame(i, debugOutput));

			foreach (var containerId in this.dockerContainerIds.Keys)
			{
				try
				{
					await this.dockerService.StopContainer(containerId);
				}
				catch (Exception ex)
				{
					this.logger.LogCritical(ex, $"Ошибка при остановке контейнера {containerId}");
				}
			}
		}

		private async Task<IReadOnlyDictionary<string, string>> Tick()
		{
			// Key - имя бота, значение - ход из контейнера
			var botOutputsTask = Task.WhenAll(this.dockerContainerIds
					.Select(async x => new { Key = x.Value, Value = await this.ReadMove(this.game.State, x.Key, x.Value)}));
			
			var botOutputs = (await botOutputsTask)
				.Where(x => x.Value != null)
				.ToDictionary(x => x.Key, x => x.Value);

			var moves = botOutputs.Select(x => x.Value.Move).ToList();
			this.game.Tick(new ReadOnlyCollection<IUserMove>(moves));

			return botOutputs
				.ToDictionary(x => x.Key, x => x.Value.DebugOutput);
		}

		private async Task<BotOutput> ReadMove(IGameState state, string containerId, string botName)
		{
      // тороплюсь, делаем "Clone" таким странным способом
		  var deserializedState = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(state));

		  foreach (var cellContentInfo in deserializedState["ContentsInfo"])
		  {
		    if (cellContentInfo["UserId"] != null && cellContentInfo.Value<string>("UserId") != botName)
		    {
		      cellContentInfo["UserId"] = Guid.NewGuid().ToString();
		    }
		  }

      var stateString = JsonConvert.SerializeObject(deserializedState) + "\n";

      // делай все правильно или умри
      try
			{
				var answer = await this.dockerService.AskContainer(containerId, stateString, this.answerDelay);
				UserAction[] actions = JsonConvert.DeserializeObject<UserAction[]>(answer.StdOut);
				return new BotOutput
				{
					Move = new UserMove(new ReadOnlyCollection<UserAction>(actions), this.dockerContainerIds[containerId]),
					DebugOutput = answer.StdErr
				};
			}
			catch (Exception ex)
			{
				this.logger.LogWarning(ex, "Ошибка при попытке получить ввод");
				
				try
				{
					await this.dockerService.StopContainer(containerId);
				}
				catch (Exception stopEx)
				{
					this.logger.LogError(stopEx, "При остановке контейнера по ошибке произошла другая ошибка");
				}
				
				this.dockerContainerIds.Remove(containerId);
			}
			
			return null;
		}

		private Frame BuildFrame(uint frameNumber, IReadOnlyDictionary<string, string> botsOutput) => new Frame
		{
			BattleId = this.battleInfo.BattleId,
			GameState = this.game.State,
			FrameNumber = frameNumber,
			DestroyedInfo = this.game.DestroyedObjects,
			BotsOutput = botsOutput
		};

		private IMapInfo LoadMap(BattleInfo loadingBattleInfo)
		{
			// FIXME код из TestConsole. Отрефакторить. 
			var data =  JsonConvert.DeserializeObject<JObject>(File.ReadAllText(Path.Combine(loadingBattleInfo.Map, "objects.json")));
			var map = new MapInfo
			{
				Height = data["Height"].Value<byte>(),
				Width = data["Width"].Value<byte>(),
			};

			List<ICellContentInfo> objects = new List<ICellContentInfo>();
			foreach (var mapObject in data["MapObjects"].Children())
			{
				var x = mapObject["Coordinates"]["X"].Value<int>();
				var y = mapObject["Coordinates"]["Y"].Value<int>();
				
				switch (mapObject["CellContentType"].Value<string>())
				{
					case "Barrier":
						byte health = mapObject["HealthCount"].Value<byte>();
						objects.Add(CellContentInfo.Barrier(x, y, health));
						break;
					
					case "NotDestroyable":
						objects.Add(CellContentInfo.Mountain(x, y));
						break;
					
					case "Spawn":
						objects.Add(CellContentInfo.Spawn(x, y));
						break;
					
					case "Water":
						objects.Add(CellContentInfo.Water(x, y));
						break;
				}
			}

			map.MapObjects = objects.AsReadOnly();
			return map;
		}

		public Battle(BattleSettings battleSettings, ISet<string> dockerImages,
			IDockerService dockerService, IStorageClient storageClient, ILogger<Battle> logger)
		{ 
			this.battleInfo = new BattleInfo
				{
					BattleId = Guid.NewGuid().ToString(),
					Map = battleSettings.Map
				};

			this.warmDelay = TimeSpan.FromSeconds(battleSettings.ContainersWarmSeconds);
			this.answerDelay = TimeSpan.FromMilliseconds(battleSettings.ContainersAnswerMilliseconds);
			
			this.dockerService = dockerService;
			this.logger = logger;
			this.storageClient = storageClient;
			this.dockerImages = dockerImages;
			var gameSettings = GameSettings.Default;
			gameSettings.ZoneRadius = battleSettings.ZoneRadius;
			this.game = new Game(dockerImages, this.LoadMap(this.battleInfo), GameSettings.Default);
		}
	}
}