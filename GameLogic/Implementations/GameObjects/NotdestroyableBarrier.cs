using GameLogic.Enums;

namespace GameLogic.Implementations.GameObjects
{
	internal sealed class NotdestroyableBarrier : NotdestroyableBase
	{
		public override CellContentType Type => CellContentType.NotDestroyable;
		protected override BulletCollisionResult BulletCollisionResult => BulletCollisionResult.Destroy;
	}
}