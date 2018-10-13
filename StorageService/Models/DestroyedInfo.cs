using System.Collections.Generic;

namespace StorageService.Models
{
	public sealed class DestroyedInfo
	{
		public IReadOnlyDictionary<string, Coordinates> DestroyedBullets { get; set; }
		public IReadOnlyCollection<Coordinates> DestroyedObjects { get; set; }
	}
}