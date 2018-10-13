using GameLogic.Interfaces.Public;

namespace TestConsole.Models
{
	internal class Settings : IGameSettings
	{
		public byte TankHealthPoint => 3;
		public byte ActionPoints => 1;
		public byte BulletActionPoints => 2;
		public byte ZoneRadius => 25;
	}
}