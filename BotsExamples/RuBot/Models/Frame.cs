namespace RuBot.Models
{
	public sealed class Frame
	{
		public string BattleId { get; set; }
		public uint FrameNumber { get; set; }
		public GameState GameState { get; set; }
		public DestroyedInfo DestroyedInfo { get; set; }
	}
}