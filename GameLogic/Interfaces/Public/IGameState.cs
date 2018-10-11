using System.Collections.Generic;

namespace GameLogic.Interfaces.Public
{
	public interface IGameState
	{
		IReadOnlyCollection<ICellContentInfo> ContentsInfo { get; }
		IReadOnlyCollection<IBulletInfo> BulletsInfo { get; }
		
		byte ZoneRadius { get; }
	}
}