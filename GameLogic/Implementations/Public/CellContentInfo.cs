using GameLogic.Enums;
using GameLogic.Interfaces.Public;

namespace GameLogic.Implementations.Public
{
	internal sealed class CellContentInfo : ICellContentInfo
	{
		public Coordinates Coordinates { get; }
		public byte HealthCount { get; }
		public CellContentType Type { get; }

		public CellContentInfo(Coordinates coordinates, byte healthCount, CellContentType type)
		{
			this.Coordinates = coordinates;
			this.HealthCount = healthCount;
			this.Type = type;
		}
	}
}