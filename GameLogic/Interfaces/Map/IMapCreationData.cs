using System.Collections.Generic;
using GameLogic.Interfaces.Public;

namespace GameLogic.Interfaces.Map
{
	internal interface IMapCreationData
	{
		IMapInfo MapInfo { get; }
		IReadOnlyCollection<ICellContent> UserContents { get; }
	}
}