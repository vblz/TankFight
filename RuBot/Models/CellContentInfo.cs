namespace RuBot.Models
{
	public sealed class CellContentInfo
	{
		public Coordinates Coordinates { get; set; }
		public byte HealthCount { get; set; }
		public CellContentType Type { get; set; }
		public string UserId { get; set; }
	}
}