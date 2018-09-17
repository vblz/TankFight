using GameLogic.Interfaces.Game;

namespace GameLogic.Implementations.GameObjects
{
	internal sealed class Health : IHealth
	{
		public bool IsAlive => this.HealthPoint > 0;
		public byte HealthPoint { get; private set; }

		public void ProcessShoot()
		{
			if (this.IsAlive)
			{
				--this.HealthPoint;
			}
		}

		public Health(byte healthPoint)
		{
			this.HealthPoint = healthPoint;
		}
	}
}