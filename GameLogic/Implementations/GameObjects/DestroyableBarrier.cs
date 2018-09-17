using GameLogic.Enums;
using GameLogic.Interfaces.Game;
using GameLogic.Interfaces.Map;

namespace GameLogic.Implementations.GameObjects
{
	
	internal sealed class DestroyableBarrier : ICellContent
	{
		private readonly IHealth health;

		public bool IsAlive => this.health.IsAlive;
		public byte HealthPoint => this.health.HealthPoint;
		
		public CellContentType Type => CellContentType.Barrier;
		
		public void ProcessShoot() => this.health.ProcessShoot();

		public DestroyableBarrier(IHealth health)
		{
			this.health = health;
		}
	}
}