using GameLogic.Enums;

namespace GameLogic.Interfaces.Map
{
	internal interface ICellContent
	{
		bool IsAlive { get; }
		byte HealthPoint { get; }
		CellContentType Type { get; }
		BulletCollisionResult ProcessShoot();
	}
}