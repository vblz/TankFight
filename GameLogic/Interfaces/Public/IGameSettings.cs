namespace GameLogic.Interfaces.Public
{
	public interface IGameSettings
	{
		byte TankHealthPoint { get; }
		byte ActionPoints { get; }
		byte BulletActionPoints { get; }
		byte ZoneRadius { get; }
	}
}