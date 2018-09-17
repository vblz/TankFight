using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GameLogic.Enums;
using GameLogic.Interfaces.Map;
using GameLogic.Interfaces.Services;

namespace GameLogic.Implementations.Services
{
	internal sealed class RandomSpawnService : ISpawnService
	{
		public void Spawn(IReadOnlyCollection<ICell> allCells, IReadOnlyCollection<ICellContent> contents)
		{
			if (allCells == null)
			{
				throw new ArgumentNullException(nameof(allCells));
			}
			
			if (contents == null)
			{
				throw new ArgumentNullException(nameof(contents));
			}

			var spawnCells = allCells
				.Where(x => x != null && !x.IsEmpty)
				.Where(x => x.Content.Type == CellContentType.Spawn)
				.ToList();
			
			if (contents.Count > spawnCells.Count)
			{
				throw new InvalidOperationException();
			}
			
			var random = new Random();
			var shuffledCells = spawnCells
				.OrderBy(x => random.Next())
				.ToList();
			
			ReplaceInCells(shuffledCells, contents);
		}
		
		/// <summary>
		/// Заменяет содержимое ячеек на переданный контент.
		/// Если ячеек больше, чем контента - очищает оставшиеся.
		/// </summary>
		/// <param name="cells">Ячейки.</param>
		/// <param name="contents">Контент</param>
		/// <exception cref="InvalidOperationException">В случае, если количество ячеек меньше количества контента.</exception>
		/// <exception cref="NullReferenceException">Если среди переданных ячеек встретился null.</exception>
		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		private static void ReplaceInCells(IReadOnlyCollection<ICell> cells, IReadOnlyCollection<ICellContent> contents)
		{
			if (contents.Count > cells.Count)
			{
				throw new InvalidOperationException();
			}

			using (var contentsEnum = contents.GetEnumerator())
			using (var cellsEnum = cells.GetEnumerator())
			{
				while (cellsEnum.MoveNext() && contentsEnum.MoveNext())
				{
					cellsEnum.Current.Pop();
					cellsEnum.Current.Put(contentsEnum.Current);
				}

				while (cellsEnum.MoveNext())
				{
					cellsEnum.Current.Pop();
				}
			}
		}
	}
}