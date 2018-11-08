namespace FightServer.Settings
{
  public class BattleSettings
  {
    public string Map { get; set; }
    public string StorageServiceLocation { get; set; }
    public byte ZoneRadius { get; set; }
    public int ContainersWarmSeconds { get; set; }
    public int ContainersAnswerMilliseconds { get; set; }
  }
}