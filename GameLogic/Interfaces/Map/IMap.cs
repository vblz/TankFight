using System.Collections.Generic;
using GameLogic.Implementations.Public;
using GameLogic.Interfaces.Public;

namespace GameLogic.Interfaces.Map
{
	internal interface IMap
	{
		ICell GetCell(Coordinates coord);
		ICell GetCell(int x, int y);
		ICell GetUserCell(string userId);
		IReadOnlyCollection<ICellContentInfo> GetState();
		void ClearDeadCells();
	}
}