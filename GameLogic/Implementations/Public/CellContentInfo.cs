using GameLogic.Enums;
using GameLogic.Implementations.GameObjects;
using GameLogic.Interfaces.Map;
using GameLogic.Interfaces.Public;

namespace GameLogic.Implementations.Public
{
	internal sealed class CellContentInfo : ICellContentInfo
	{
		public Coordinates Coordinates { get; }
		public byte HealthCount { get; }
		public CellContentType Type { get; }
		public string UserId { get; set; }

		public CellContentInfo(Coordinates coordinates, byte healthCount, CellContentType type, string userId)
		{
			this.Coordinates = coordinates;
			this.HealthCount = healthCount;
			this.Type = type;
			this.UserId = userId;
		}
		
		public CellContentInfo(ICell cell) : this(cell.Coordinates,
			cell.Content.HealthPoint,
			cell.Content.Type,
			(cell.Content as Tank)?.UserId)
		{}
	}
}