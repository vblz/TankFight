using GameLogic.Interfaces.Public;

namespace TestConsole.Models.StorageService
{
	public sealed class Frame
	{
		public string BattleId { get; set; }
		public uint FrameNumber { get; set; }
		public IGameState GameState { get; set; }
		public IDestroyedInfo DestroyedInfo { get; set; }
	}
}