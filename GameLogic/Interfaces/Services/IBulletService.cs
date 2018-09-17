using System.Collections.Generic;
using GameLogic.Enums;
using GameLogic.Interfaces.Public;

namespace GameLogic.Interfaces.Services
{
	internal interface IBulletService
	{
		IReadOnlyCollection<IBulletInfo> Bullets { get; }
		
		void Process();
		void CreateBullet(string userId,  Direction direction);
	}
}