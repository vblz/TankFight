using System.Collections.Generic;
using GameLogic.Interfaces.Public;

namespace FightServer.Models
{
	public sealed class Frame
	{
		public string BattleId { get; set; }
		public uint FrameNumber { get; set; }
		public IGameState GameState { get; set; }
		public IDestroyedInfo DestroyedInfo { get; set; }
		public IReadOnlyDictionary<string, string> BotsOutput { get; set; }
	}
}