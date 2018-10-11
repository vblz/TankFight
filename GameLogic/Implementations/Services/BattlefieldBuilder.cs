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
				if (map[i] != null)
				{
					continue;
				}
				
				map[i] = new Cell(Coordinates.FromIndex(i, width));
				
				// у карты по границе или уже стоят неубиваемые блоки или их надо расставить
				if (IsIndexOnBorder(i, map.Length, width))
				{
					map[i].Put(new NotdestroyableBarrier());
				}
			}

			return new Battlefield(map, width);
		}

		private static bool IsIndexOnBorder(int index, int cellsCount, int widht)
		{
			return index < widht // верхняя
			       || index >= cellsCount - widht // нижняя
			       || index % widht == 0 // левая
			       || (index + 1) % widht == 0; // правая
		}

		private static bool IsCellNotDestroyable(ICell cell)
		{
			if (cell == null || cell.Content == null)
			{
				return false;
			}
			
			return cell?.Content?.Type != CellContentType.NotDestroyable;
		}

		private static bool IsBordered(IReadOnlyList<ICell> map, int width)
		{
			// верх и низ, лево и право равны между собой, поэтому пробегаемся в одном цикле
			
			// горизонтали
			for (int top = 0, bottom = map.Count - width; top < width; ++top, ++bottom)
			{
				if (!IsCellNotDestroyable(map[top]) || !IsCellNotDestroyable(map[bottom]))
				{
					return false;
				}
			}
			
			// вертикали, 0 и последнюю строчку не проверяем, так как проверили выше
			for (int left = width, right = width * 2 - 1; left < map.Count - width; left += width, right += width)
			{
				if (!IsCellNotDestroyable(map[left]) || !IsCellNotDestroyable(map[right]))
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
					
					case CellContentType.Water:
						content = new Water();
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