using GameLogic.Enums;

namespace GameLogic.Implementations.GameObjects
{
	internal sealed class Water : NotdestroyableBase
	{
		public override CellContentType Type => CellContentType.Water;
		protected override BulletCollisionResult BulletCollisionResult => BulletCollisionResult.Pass;
	}
}