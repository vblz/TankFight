using GameLogic.Enums;
using GameLogic.Implementations.Public;

namespace GameLogic.Implementations.GameObjects
{
	internal sealed class Bullet
	{
		public Direction Direction { get; }
		public Coordinates Coordinates { get; private set; }

		public void Move()
		{
			this.Coordinates = this.Coordinates.GetNeighborCoordinates(this.Direction);
		}
		
		public Bullet(Direction direction, Coordinates coordinates)
		{
			this.Direction = direction;
			this.Coordinates = coordinates;
		}
	}
}