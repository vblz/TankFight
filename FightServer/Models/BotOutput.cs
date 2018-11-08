using GameLogic.Interfaces.Public;

namespace FightServer.Models
{
  public class BotOutput
  {
    public IUserMove Move { get; set; }
    public string DebugOutput { get; set; }
  }
}