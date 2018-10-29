using System.Collections.Generic;
using GameLogic.Enums;
using GameLogic.Implementations.Public;
using GameLogic.Interfaces.Game;
using GameLogic.Interfaces.Map;
using GameLogic.Interfaces.Public;

namespace GameLogic.Interfaces.Services
{
	internal interface IMapAdapter
	{
		byte Height { get; }
		byte Width { get; }
		
		IMoveDirection MoveDirection(string fromUserId, Direction direction);
		ICell GetCell(Coordinates coordinates);
		IReadOnlyCollection<ICellContentInfo> GetState();
		IReadOnlyCollection<ICellContentInfo> ClearDeadCells();
	}
}