using System;
using GameLogic.Enums;

namespace GameLogic.Implementations.Public
{
	public struct Coordinates : IEquatable<Coordinates>
	{
		public int X { get; }
		public int Y { get; }
		
		public bool Equals(Coordinates other)
		{
			return this.X == other.X && this.Y == other.Y;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is Coordinates coordinates && this.Equals(coordinates);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (this.X * 397) ^ this.Y;
			}
		}

		public static bool operator ==(Coordinates c1, Coordinates c2) 
		{
			return c1.Equals(c2);
		}

		public static bool operator !=(Coordinates c1, Coordinates c2)
		{
			return !(c1 == c2);
		}

		public Coordinates GetNeighborCoordinates(Direction direction)
		{
			var xDelta = 0;
			var yDelta = 0;

			switch (direction)
			{
				case Direction.Down:
					--yDelta;
					break;
				case Direction.Up:
					++yDelta;
					break;
          
				case Direction.Right:
					++xDelta;
					break;
          
				case Direction.Left:
					--xDelta;
					break;
				
				default:
					throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}

			return new Coordinates(this.X + xDelta, this.Y + yDelta);
		}

		public static Coordinates FromIndex(int index, int width)
		{
			return new Coordinates(index % width, index / width);
		}

		public Coordinates(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}
	}
}