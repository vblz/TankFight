using System;
using System.Collections.Generic;
using GameLogic.Enums;
using GameLogic.Implementations.GameObjects;
using GameLogic.Implementations.Map;
using GameLogic.Implementations.Public;
using GameLogic.Interfaces.Map;
using GameLogic.Interfaces.Public;
using GameLogic.Interfaces.Services;

namespace GameLogic.Implementations.Services
{
	internal sealed class BattlefieldBuilder : IBattlefieldBuilder
	{
		public IBattlefield Build(IMapInfo mapInfo)
		{
			byte width = mapInfo.Width;
			var map = FillMap(mapInfo, 0);
			
			// если не окружено блоками неубиваемыми - создаем больше на один, туда поместим блоки
			if (!IsBordered(map, width))
			{
				map = FillMap(mapInfo, 1);
				width += 2;
			}

			for (int i = 0; i < map.Length; ++i)
			{
				if (map[i] != null) continue;
				// у карты по границе или уже стоят неубиваемые блоки или их надо расставить
				if (IsBorder(i, map.Length, width))
				{
					map[i] = new Cell(new NotdestroyableBarrier(), Coordinates.FromIndex(i, width));
				}
				else
				{
					map[i] = new Cell(Coordinates.FromIndex(i, width));
				}
			}

			return new Battlefield(map, width);
		}

		private static bool IsBorder(int index, int cellsCount, int widht)
		{
			return index < widht // верхняя
			       || index >= cellsCount - widht // нижняя
			       || index % widht == 0 // левая
			       || (index + 1) % widht == 0; // правая
		}

		private static bool IsBordered(IReadOnlyList<ICell> map, int width)
		{
			for (int i = 0; i < width; ++i)
			{
				if (map[i]?.Content?.Type != CellContentType.NotDestroyable)
				{
					return false;
				}
			}

			for (int i = map.Count - width; i < map.Count; ++i)
			{
				if (map[i]?.Content?.Type != CellContentType.NotDestroyable)
				{
					return false;
				}
			}

			for (int i = width; i < map.Count; i += width)
			{
				if (map[i]?.Content?.Type != CellContentType.NotDestroyable)
				{
					return false;
				}
			}

			for (int i = width * 2 - 1; i < map.Count; i += width)
			{
				if (map[i]?.Content?.Type != CellContentType.NotDestroyable)
				{
					return false;
				}
			}

			return true;
		}

		private static ICell[] FillMap(IMapInfo mapInfo, int increaseByEachSide)
		{
			var result = new ICell[(mapInfo.Width + increaseByEachSide * 2) * (mapInfo.Height + increaseByEachSide * 2)];
			var mapWidth = mapInfo.Width + increaseByEachSide * 2;

			foreach (var cell in mapInfo.MapObjects)
			{
				var cellCoordinates = new Coordinates(cell.Coordinates.X + increaseByEachSide,
					cell.Coordinates.Y + increaseByEachSide);
				
				int newIndex = cellCoordinates.X + cellCoordinates.Y * mapWidth;

				ICellContent content;
				switch (cell.Type)
				{
					case CellContentType.Barrier:
						content = new DestroyableBarrier(new Health(cell.HealthCount));
						break;
					
					case CellContentType.NotDestroyable:
						content = new NotdestroyableBarrier();
						break;
					
					case CellContentType.Spawn:
						content = new SpawnMark();
						break;
					
					case CellContentType.Tank:
						throw new ArgumentException("Не возможно установить танк при генерации поля боя.");
					
					default:
						throw new ArgumentOutOfRangeException();
				}
				
				result[newIndex] = new Cell(content, cellCoordinates);
			}

			return result;
		}
	}
}