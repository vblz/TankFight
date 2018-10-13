using System.Collections.Generic;

namespace StorageService.Models
{
	public sealed class GameState
	{
		public IReadOnlyCollection<CellContentInfo> ContentsInfo { get; set; }
		public IReadOnlyCollection<BulletInfo> BulletsInfo { get; set; }
		public byte ZoneRadius { get; set; }
	}
}