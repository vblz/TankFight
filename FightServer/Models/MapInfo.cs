using System.Collections.Generic;
using GameLogic.Interfaces.Public;

namespace FightServer.Models
{
	internal sealed class MapInfo : IMapInfo
	{
		public IReadOnlyCollection<ICellContentInfo> MapObjects { get; set; }
		public byte Height { get; set; }
		public byte Width { get; set; }
	}
}