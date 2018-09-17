using GameLogic.Enums;
using GameLogic.Implementations.Public;
using GameLogic.Interfaces.Public;

namespace TestConsole
{
	internal class CellContentInfo : ICellContentInfo
	{
		public Coordinates Coordinates { get; private set; }
		public byte HealthCount { get; private  set; }
		public CellContentType Type { get; private  set; }

		public static CellContentInfo Mountain(int x, int y)
		{
			return new CellContentInfo
			{
				Coordinates = new Coordinates(x, y),
				HealthCount = 0,
				Type = CellContentType.NotDestroyable
			};
		}

		public static CellContentInfo Mountain(Coordinates coord)
		{
			return Mountain(coord.X, coord.Y);
		}

		public static CellContentInfo Spawn(int x, int y)
		{
			return new CellContentInfo
			{
				Coordinates = new Coordinates(x, y),
				HealthCount = 0,
				Type = CellContentType.Spawn
			};
		}

		public static CellContentInfo House(int x, int y)
		{
			return new CellContentInfo
			{
				Coordinates = new Coordinates(x, y),
				HealthCount = 1,
				Type = CellContentType.Barrier
			};
		}
	}
}