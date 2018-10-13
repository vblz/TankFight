using GameLogic.Enums;
using GameLogic.Implementations.Public;
using GameLogic.Interfaces.Public;

namespace TestConsole.Models
{
	internal class CellContentInfo : ICellContentInfo
	{
		public Coordinates Coordinates { get; private set; }
		public byte HealthCount { get; private  set; }
		public CellContentType Type { get; private  set; }
		public string UserId { get; set; }

		public static CellContentInfo Mountain(int x, int y)
		{
			return new CellContentInfo
			{
				Coordinates = new Coordinates(x, y),
				HealthCount = 0,
				Type = CellContentType.NotDestroyable
			};
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

		public static CellContentInfo Barrier(int x, int y, byte health)
		{
			return new CellContentInfo
			{
				Coordinates = new Coordinates(x, y),
				HealthCount = health,
				Type = CellContentType.Barrier
			};
		}

		public static CellContentInfo Water(int x, int y)
		{
			return new CellContentInfo
			{
				Coordinates = new Coordinates(x, y),
				HealthCount = 0,
				Type = CellContentType.Water
			};
		}
	}
}