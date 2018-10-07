using System.Collections.Generic;
using System.Collections.ObjectModel;
using GameLogic.Interfaces.Public;

namespace GameLogic.Implementations.Public
{
	internal sealed class DestroyedInfo : IDestroyedInfo
	{
		public IReadOnlyDictionary<string, Coordinates> DestroyedBullets { get; }
		public IReadOnlyCollection<Coordinates> DestroyedObjects { get; }

		public DestroyedInfo(IDictionary<string, Coordinates> destroyedBullets,
			IList<Coordinates> destroyedObjects)
		{
			this.DestroyedBullets = new ReadOnlyDictionary<string, Coordinates>(destroyedBullets);
			this.DestroyedObjects = new ReadOnlyCollection<Coordinates>(destroyedObjects);
		}
	}
}