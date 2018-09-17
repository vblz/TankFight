using GameLogic.Enums;
using GameLogic.Interfaces.Services;

namespace GameLogic.Implementations.Services
{
	internal sealed class MoveService : IMoveService
	{
		private readonly IMapAdapter mapAdapter;

		public void Move(string userId, Direction direction)
		{
			var moveDirection = this.mapAdapter.MoveDirection(userId, direction);
			if (moveDirection.ToCell.IsEmpty)
			{
				moveDirection.ToCell.Put(moveDirection.UserCell.Pop());
			}
		}

		public MoveService(IMapAdapter mapAdapter)
		{
			this.mapAdapter = mapAdapter;
		}
	}
}