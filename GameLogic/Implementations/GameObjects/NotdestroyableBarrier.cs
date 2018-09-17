using GameLogic.Enums;
using GameLogic.Interfaces.Map;

namespace GameLogic.Implementations.GameObjects
{
	internal sealed class NotdestroyableBarrier : ICellContent
	{
		public bool IsAlive => true;
		public byte HealthPoint => byte.MaxValue;
		public CellContentType Type => CellContentType.NotDestroyable;
		public void ProcessShoot() { /*"Кто его посадит? Он же памятник"*/ }
	}
}