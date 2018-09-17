using GameLogic.Enums;
using GameLogic.Interfaces.Game;
using GameLogic.Interfaces.Map;

namespace GameLogic.Implementations.GameObjects
{
	internal sealed class Tank : ICellContent
	{
		private readonly IHealth health;
		public bool IsAlive => this.health.IsAlive;
		public byte HealthPoint => this.health.HealthPoint;
		
		public CellContentType Type => CellContentType.Tank;
		
		public string UserId { get; }
		
		public void ProcessShoot() => this.health.ProcessShoot();

		public Tank(IHealth health, string userId)
		{
			this.health = health;
			this.UserId = userId;
		}
	}
}