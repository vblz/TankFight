using GameLogic.Enums;
using GameLogic.Interfaces.Map;

namespace GameLogic.Implementations.GameObjects
{
	internal abstract class NotdestroyableBase : ICellContent
	{
		public bool IsAlive => true;
		public byte HealthPoint => byte.MaxValue;
		
		public abstract CellContentType Type { get; }

		protected abstract BulletCollisionResult BulletCollisionResult { get; }
		
		public BulletCollisionResult ProcessShoot() => this.BulletCollisionResult;
	}
}