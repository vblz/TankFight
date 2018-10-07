using System.Collections.Generic;
using GameLogic.Enums;
using GameLogic.Interfaces.Public;

namespace GameLogic.Interfaces.Services
{
	internal interface IBulletService
	{
		IReadOnlyCollection<IBulletInfo> Bullets { get; }
		
		
		IDestroyedInfo Process();
		void UserShoot(string userId,  Direction direction);
	}
}