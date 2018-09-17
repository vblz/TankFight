using GameLogic.Enums;
using GameLogic.Interfaces.Public;

namespace GameLogic.Implementations.Public
{
	internal sealed class BulletInfo : IBulletInfo
	{
		public Coordinates Coordinates { get; }
		public Direction Direction { get; }
		public string OwnerId { get; }

		public BulletInfo(Coordinates coordinates, Direction direction, string ownerId)
		{
			this.Coordinates = coordinates;
			this.Direction = direction;
			this.OwnerId = ownerId;
		}
	}
}