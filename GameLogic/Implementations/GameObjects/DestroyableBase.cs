using GameLogic.Enums;
using GameLogic.Interfaces.Game;
using GameLogic.Interfaces.Map;

namespace GameLogic.Implementations.GameObjects
{
	internal abstract class DestroyableBase : ICellContent
	{
		private readonly IHealth health;

		public bool IsAlive => this.health.IsAlive;
		public byte HealthPoint => this.health.HealthPoint;
		
		public abstract CellContentType Type { get; }
		
		public BulletCollisionResult ProcessShoot()
		{
			this.health.ProcessShoot();
			
			return BulletCollisionResult.Destroy;
		}

		public DestroyableBase(IHealth health)
		{
			this.health = health;
		}
	}
}