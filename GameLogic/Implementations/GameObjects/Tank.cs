using GameLogic.Enums;
using GameLogic.Interfaces.Game;

namespace GameLogic.Implementations.GameObjects
{
	internal sealed class Tank : DestroyableBase
	{
		public override CellContentType Type => CellContentType.Tank;
		
		public string UserId { get; }

		public Tank(IHealth health, string userId) : base(health)
		{
			this.UserId = userId;
		}
	}
}