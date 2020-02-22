using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLogic.Enums;
using GameLogic.Implementations.Game;
using GameLogic.Implementations.Public;
using GameLogic.Interfaces.Public;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refit;
using TestConsole.HttpClients;
using TestConsole.Models;
using TestConsole.Models.StorageService;

namespace TestConsole
{
	internal static class Program
	{
		private const int Width = 34;
		private const int Height = 20;

		private async static Task Main()
		{
			var game = new Game(new[] { "1", "2" }, GenerateMapInfo(), new Settings());

			var storageClient = RestService.For<IStorageClient>("http://localhost:5005/");

			string battleId = Guid.NewGuid().ToString();
			await storageClient.StartNewBattle(new BattleInfo { BattleId = battleId, Map = @"D:/maps/1/" });

			uint i = 0;

			while (true)
			{
				await storageClient.AddFrame(new Frame
				{
					BattleId = battleId,
					FrameNumber = i,
					DestroyedInfo = game.DestroyedObjects,
					GameState = game.State
				}, battleId);
				
				Console.Clear();
				Console.WriteLine(RenderGame(game.State));
				
				var moves = ReadMoves();
				if (moves == null)
				{
					break;
				}
				
				game.Tick(moves);
				++i;
			}
		}

		private static IUserMove[] ReadMoves()
		{
			IAction action;
			while (true)
			{
				Direction direction;
				UserActionType actionType;
				
				switch (Console.ReadKey().Key)
				{
					case ConsoleKey.Escape:
						return null;
					case ConsoleKey.W:
						direction = Direction.Up;
						actionType = UserActionType.Move;
						break;
					case ConsoleKey.A:
						direction = Direction.Left;
						actionType = UserActionType.Move;
						break;
					case ConsoleKey.S:
						direction = Direction.Down;
						actionType = UserActionType.Move;
						break;
					case ConsoleKey.D:
						direction = Direction.Right;
						actionType = UserActionType.Move;
						break;
					case ConsoleKey.UpArrow:
						direction = Direction.Up;
						actionType = UserActionType.Shoot;
						break;
					case ConsoleKey.LeftArrow:
						direction = Direction.Left;
						actionType = UserActionType.Shoot;
						break;
					case ConsoleKey.DownArrow:
						direction = Direction.Down;
						actionType = UserActionType.Shoot;
						break;
					case ConsoleKey.RightArrow:
						direction = Direction.Right;
						actionType = UserActionType.Shoot;
						break;
					
					default:
						continue;
				}

				action = new UserAction { Direction = direction, Type = actionType };
				break;
			}

			return new IUserMove[]
			{
				new UserMove(new[] { action })
			};
		}

		private static IMapInfo GenerateMapInfo()
		{
			var data =  JsonConvert.DeserializeObject<JObject>(File.ReadAllText(@"d:/maps/1/objects.json"));
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

		private static string RenderGame(IGameState state)
		{
			var sb = new StringBuilder(Width * Height + Height * Environment.NewLine.Length);
			sb.AppendFormat("Zone R: {0}", state.ZoneRadius);
			for (int y = Height; y >=0; --y)
			{
				for (int x = 0; x < Width; ++x)
				{
					char addedChar;

					var currentCoordinates = new Coordinates(x, y);
					var cell = state.ContentsInfo.SingleOrDefault(c => c.Coordinates == currentCoordinates);
					var bullet = state.BulletsInfo.SingleOrDefault(b => b.Coordinates == currentCoordinates);

					if (cell == null)
					{
						addedChar = bullet == null ? ' ' : '′';
					}
					else
					{
						switch (cell.Type)
						{
							case CellContentType.Tank:
								addedChar = cell.HealthCount.ToString()[0];
								break;
							case CellContentType.Barrier:
								addedChar = cell.HealthCount == 2 ? 'h' : 't';
								break;
							case CellContentType.NotDestroyable:
								addedChar = 'n';
								break;
							case CellContentType.Spawn:
								addedChar = 's';
								break;
							case CellContentType.Water:
								addedChar = 'w';
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}

						if (bullet != null)
						{
							addedChar = char.ToUpper(addedChar);
						}
					}

					sb.Append(addedChar);
				}

				sb.AppendLine();
			}

			return sb.ToString();
		}
	}
}