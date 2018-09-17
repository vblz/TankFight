using System.Collections.Generic;
using GameLogic.Interfaces.Map;

namespace GameLogic.Implementations.Map
{
	internal sealed class Battlefield : IBattlefield
	{
		public IReadOnlyList<ICell> Cells { get; }
		public byte Width { get; }
		
		public Battlefield(IReadOnlyList<ICell> cells, byte width)
		{
			this.Cells = cells;
			this.Width = width;
		}
	}
}