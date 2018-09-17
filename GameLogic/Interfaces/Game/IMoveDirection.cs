using GameLogic.Interfaces.Map;

namespace GameLogic.Interfaces.Game
{
	internal interface IMoveDirection
	{
		ICell UserCell { get; }
		ICell ToCell { get; }
	}
}