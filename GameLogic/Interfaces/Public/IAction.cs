using GameLogic.Enums;

namespace GameLogic.Interfaces.Public
{
	public interface IAction
	{
		UserActionType Type { get; }
		Direction Direction { get; }
	}
}