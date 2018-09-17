using GameLogic.Enums;

namespace GameLogic.Interfaces.Services
{
	internal interface IMoveService
	{
		void Move(string userId, Direction direction);
	}
}