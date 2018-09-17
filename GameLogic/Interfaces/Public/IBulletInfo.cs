using GameLogic.Enums;
using GameLogic.Implementations.Public;

namespace GameLogic.Interfaces.Public
{
	public interface IBulletInfo
	{
		Coordinates Coordinates { get; }
		Direction Direction { get; }
		string OwnerId { get; }
	}
}