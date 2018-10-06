using GameLogic.Enums;
using GameLogic.Interfaces.Map;

namespace GameLogic.Implementations.GameObjects
{
	internal sealed class SpawnMark : ICellContent
	{
		public bool IsAlive => false;
		public byte HealthPoint => 0;
		public CellContentType Type => CellContentType.Spawn;
		
		public BulletCollisionResult ProcessShoot()
		{
			throw new System.NotSupportedException();
		}
	}
}