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
using GameLogic.Implementations.Game;
using GameLogic.Interfaces.Public;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FightServer.Services.Implementations
{
	internal sealed class Battle
	{
		private readonly BattleInfo battleInfo;
		private readonly IDockerService dockerService;
		private readonly ICollection<string> dockerImages;
		private readonly Game game;
		private readonly ILogger<Battle> logger;
		private readonly IStorageClient storageClient;
		
		private IDictionary<string, string> dockerContainerIds;

		private async Task StartContainers()
		{
			this.dockerContainerIds = new Dictionary<string, string>(this.dockerImages.Count);
			foreach (var dockerImage in this.dockerImages)
			{
				var containerId = await this.dockerService.StartContainer(dockerImage);
				this.dockerContainerIds.Add(containerId, dockerImage);
			}
		}
		

		public async Task Start(CancellationToken cancellationToken)
		{
			await this.storageClient.StartNewBattle(this.battleInfo);
			
			await this.StartContainers();

			uint i = 0;
			while (!cancellationToken.IsCancellationRequested && !this.game.IsEnded())
			{
				await this.storageClient.AddFrame(this.battleInfo.BattleId, new Frame
				{
					BattleId = this.battleInfo.BattleId,
					GameState = this.game.State,
					FrameNumber = i++,
					DestroyedInfo = this.game.DestroyedObjects
				});
				
				await this.Tick();
			}
			
			await this.storageClient.AddFrame(this.battleInfo.BattleId, new Frame
			{
				BattleId = this.battleInfo.BattleId,
				GameState = this.game.State,
				FrameNumber = i++,
				DestroyedInfo = this.game.DestroyedObjects
			});

			foreach (var containerId in this.dockerContainerIds.Keys)
			{
				await this.dockerService.StopContainer(containerId);
			}
		}

		private async Task Tick()
		{
			var state = JsonConvert.SerializeObject(this.game.State);
			var movesTasks = this.dockerContainerIds
				.Select(x => this.ReadMove(state, x.Key));
			
			var moves = (await Task.WhenAll(movesTasks))
				.Where(x => x != null);
			
			this.game.Tick(new ReadOnlyCollection<IUserMove>(moves.ToList()));
		}

		private async Task<IUserMove> ReadMove(string state, string containerId)
		{
			// делай все правильно или умри
			try
			{
				var answer = await this.dockerService.AskContainer(containerId, state);
				UserAction[] actions = JsonConvert.DeserializeObject<UserAction[]>(answer);
				return new UserMove(new ReadOnlyCollection<IAction>(actions), this.dockerContainerIds[containerId]);
			}
			catch (Exception ex)
			{
				this.logger.LogInformation(ex, "Ошибка при попытке получить ввод");
				await this.dockerService.StopContainer(containerId);
				this.dockerContainerIds.Remove(containerId);
			}
			
			return null;
		}

		private IMapInfo LoadMap(BattleInfo battleInfo)
		{
			// FIXME код из TestConsole. Отрефакторить. 
			var data =  JsonConvert.DeserializeObject<JObject>(File.ReadAllText(Path.Combine(battleInfo.Map, "objects.json")));
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

			objects.Add(CellContentInfo.Spawn(1, 1));
			objects.Add(CellContentInfo.Spawn(1, 2));

			map.MapObjects = objects.AsReadOnly();
			return map;
		}

		public Battle(BattleInfo battleInfo, ICollection<string> dockerImages,
			IDockerService dockerService, IStorageClient storageClient, ILogger<Battle> logger)
		{
			this.battleInfo = battleInfo;
			this.dockerService = dockerService;
			this.logger = logger;
			this.storageClient = storageClient;
			this.dockerImages = dockerImages;
			this.game = new Game(dockerImages, this.LoadMap(battleInfo), GameSettings.Default);
		}
	}
}