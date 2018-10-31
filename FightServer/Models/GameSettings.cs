using GameLogic.Interfaces.Public;

namespace FightServer.Models
{
	internal sealed class GameSettings : IGameSettings
	{
		public static GameSettings Default = new GameSettings
		{
			TankHealthPoint = 3,
			ZoneRadius = byte.MaxValue,
			ActionPoints = 1,
			BulletActionPoints = 2
		};
		
		public byte TankHealthPoint { get; set; }
		public byte ActionPoints { get;  set; }
		public byte BulletActionPoints { get;  set; }
		public byte ZoneRadius { get;  set; }
	}
}