using GameLogic.Enums;
using GameLogic.Interfaces.Game;

namespace GameLogic.Implementations.GameObjects
{
	internal sealed class DestroyableBarrier : DestroyableBase
	{
		public override CellContentType Type => CellContentType.Barrier;

		public DestroyableBarrier(IHealth health) : base(health)
		{
		}
	}
}