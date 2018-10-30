using System.Collections.Generic;
using System.Linq;
using GameLogic.Enums;
using GameLogic.Implementations.Public;
using GameLogic.Interfaces.Public;
using GameLogic.Interfaces.Services;

namespace GameLogic.Implementations.Services
{
	internal sealed class ZoneService : IZoneService
	{
		private readonly IMapAdapter mapAdapter;

		public byte Radius { get; private set; }
		
		public IReadOnlyCollection<ICellContentInfo> Process()
		{
			var result = this.Damage();
			if (this.Radius > 0)
			{
				--this.Radius;
			}

			return result;
		}

		private IReadOnlyCollection<ICellContentInfo> Damage()
		{
			var (xLess, xMore) = this.CalcAxisRect(this.mapAdapter.Width);
			var (yLess, yMore) = this.CalcAxisRect(this.mapAdapter.Height);

			var coordsToDamage = this.mapAdapter.GetState()
				.Where(cellContentInfo => cellContentInfo.Type == CellContentType.Tank)
				.Where(cellContentInfo => cellContentInfo.HealthCount > 0)
				.Where(cellContentInfo => IsOutside(cellContentInfo.Coordinates, xLess, xMore, yLess, yMore))
				.Select(cellContentInfo => cellContentInfo.Coordinates);

			foreach (var coord in coordsToDamage)
			{
				this.mapAdapter.GetCell(coord).Content?.ProcessShoot();
			}

			return this.mapAdapter.ClearDeadCells();
		}

		private static bool IsOutside(Coordinates coord, int xLess, int xMore, int yLess, int yMore)
		{
			return coord.X < xLess || coord.X > xMore || coord.Y < yLess || coord.Y > yMore;
		}

		private (int, int) CalcAxisRect(byte size)
		{
			return (size / 2 - this.Radius + size % 2,
				size / 2 + this.Radius - 1);
		}

		public ZoneService(IMapAdapter mapAdapter, byte radius)
		{
			this.mapAdapter = mapAdapter;
			this.Radius = radius;
		}
	}
}