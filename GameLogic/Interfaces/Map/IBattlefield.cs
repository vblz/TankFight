using System.Collections.Generic;

namespace GameLogic.Interfaces.Map
{
	internal interface IBattlefield
	{
		IReadOnlyList<ICell> Cells { get; }
		byte Width { get; }
	}
}