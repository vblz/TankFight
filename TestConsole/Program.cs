using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLogic.Enums;
using GameLogic.Implementations.Game;
using GameLogic.Implementations.Public;
using GameLogic.Interfaces.Public;

namespace TestConsole
{
	internal static class Program
	{
		private const int Width = 34;
		private const int Height = 20;

		private static void Main()
		{
			var game = new Game(new[] { "1", "2" }, GenerateMapInfo(), new Settings());

			while (true)
			{
				Console.Clear();
				Console.WriteLine(RenderGame(game.State));
				
				var moves = ReadMoves();
				if (moves == null)
				{
					break;
				}
				
				game.Tick(moves);
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
			var map = new MapInfo
			{
				Height = Height,
				Width = Width
			};

			var objects = new List<ICellContentInfo>();

			for (int i = 0; i < Width; ++i)
			{
				objects.Add(CellContentInfo.Mountain(Coordinates.FromIndex(i, Width)));
			}

			for (int i = (Height - 1) * Width; i < Width * Height; ++i)
			{
				objects.Add(CellContentInfo.Mountain(Coordinates.FromIndex(i, Width)));
			}

			for (int i = Width; i < Width * Height; i += Width)
			{
				objects.Add(CellContentInfo.Mountain(Coordinates.FromIndex(i, Width)));
			}

			for (int i = Width * 2 - 1; i < Width * Height; i += Width)
			{
				objects.Add(CellContentInfo.Mountain(Coordinates.FromIndex(i, Width)));
			}

			for (int i = 6; i < 16; ++i)
			{
				objects.Add(CellContentInfo.Mountain(1, i));
			}

			objects.Add(CellContentInfo.House(2, 3));
			objects.Add(CellContentInfo.Spawn(10, 10));
			objects.Add(CellContentInfo.Spawn(12, 12));

			map.MapObjects = objects.ToArray();
			return map;
		}

		private static string RenderGame(IGameState state)
		{
			var sb = new StringBuilder(Width * Height + Height * Environment.NewLine.Length);
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
								addedChar = 'b';
								break;
							case CellContentType.NotDestroyable:
								addedChar = 'n';
								break;
							case CellContentType.Spawn:
								addedChar = 's';
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