using System.Collections.Generic;
using GameLogic.Implementations.Public;

namespace GameLogic.Interfaces.Public
{
	public interface IDestroyedInfo
	{
		IReadOnlyDictionary<string, Coordinates> DestroyedBullets { get; }
		IReadOnlyCollection<ICellContentInfo> DestroyedObjects { get; }
	}
}