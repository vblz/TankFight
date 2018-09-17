using GameLogic.Interfaces.Game;
using GameLogic.Interfaces.Map;

namespace GameLogic.Implementations.Game
{
	internal sealed class MoveDirection : IMoveDirection
	{
		public ICell UserCell { get; }
		public ICell ToCell { get; }
		
		public MoveDirection(ICell userCell, ICell toCell)
		{
			this.UserCell = userCell;
			this.ToCell = toCell;
		}
	}
}