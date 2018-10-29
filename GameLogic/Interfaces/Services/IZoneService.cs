using System.Collections.Generic;
using GameLogic.Interfaces.Public;

namespace GameLogic.Interfaces.Services
{
	internal interface IZoneService
	{
		byte Radius { get; }
		
		IReadOnlyCollection<ICellContentInfo> Process();
	}
}