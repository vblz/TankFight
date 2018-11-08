using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace StorageService.Models
{
	[BsonIgnoreExtraElements]
	public sealed class Frame
	{
		public string BattleId { get; set; }
		public uint FrameNumber { get; set; }
		public GameState GameState { get; set; }
		public DestroyedInfo DestroyedInfo { get; set; }
		public Dictionary<string, string> BotsOutput { get; set; }
	}
}