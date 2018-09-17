namespace GameLogic.Interfaces.Game
 {
 	internal interface IHealth
 	{
 		bool IsAlive { get; }
 		byte HealthPoint { get; }
 		void ProcessShoot();
 	}
 }