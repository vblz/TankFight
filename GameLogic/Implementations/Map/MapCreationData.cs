using System.Collections.Generic;
using GameLogic.Interfaces.Map;
using GameLogic.Interfaces.Public;

namespace GameLogic.Implementations.Map
{
	internal sealed class MapCreationData : IMapCreationData
	{
		public IMapInfo MapInfo { get; }
		public IReadOnlyCollection<ICellContent> UserContents { get; }
		
		public MapCreationData(IMapInfo mapInfo, IReadOnlyCollection<ICellContent> userContents)
		{
			this.MapInfo = mapInfo;
			this.UserContents = userContents;
		}
	}
}