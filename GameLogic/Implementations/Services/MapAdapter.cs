using System;
using System.Collections.Generic;
using GameLogic.Enums;
using GameLogic.Implementations.Exceptions;
using GameLogic.Implementations.Game;
using GameLogic.Implementations.Public;
using GameLogic.Interfaces.Game;
using GameLogic.Interfaces.Map;
using GameLogic.Interfaces.Public;
using GameLogic.Interfaces.Services;

namespace GameLogic.Implementations.Services
{
	internal sealed class MapAdapter : IMapAdapter
	{
		private readonly IMap map;

		public byte Height { get; }
		public byte Width { get; }

		public IMoveDirection MoveDirection(string userId, Direction direction)
		{
			if (string.IsNullOrEmpty(userId))
			{
				throw new ArgumentException("Передан пустой идентификатор пользователя", nameof(userId));
			}
			var userCell = this.map.GetUserCell(userId);
			if (userCell == null)
			{
				throw new UserNotFoundException("Невозможно найти несуществующий танк.");
			}

			if (userCell.IsEmpty
			    || userCell.Content.Type != CellContentType.Tank
			    || !userCell.Content.IsAlive)
			{
				throw new InvalidOperationException("Вернулся не верный тип содержимого ячейки" +
				                                    " по идентификатору пользователя.");
			}

			var toCoords = userCell.Coordinates.GetNeighborCoordinates(direction);
			var toCell = this.map.GetCell(toCoords);
			if (toCell == null)
			{
				throw new InvalidOperationException("Невозможная клетка направления");
			}

			return new MoveDirection(userCell, toCell);
		}

		public ICell GetCell(Coordinates coordinates) => this.map.GetCell(coordinates);

		public IReadOnlyCollection<ICellContentInfo> GetState() => this.map.GetState();

		public IReadOnlyCollection<ICellContentInfo> ClearDeadCells() => this.map.ClearDeadCells();

		public MapAdapter(IMap map)
		{
			this.map = map;
			this.Height = map.Height;
			this.Width = map.Width;
		}
	}
}