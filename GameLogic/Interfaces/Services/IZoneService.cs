using System.Collections.Generic;
using GameLogic.Implementations.Public;

namespace GameLogic.Interfaces.Services
{
	internal interface IZoneService
	{
		byte Radius { get; }
		
		IReadOnlyCollection<Coordinates> Process();
	}
}