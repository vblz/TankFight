using System.Collections.Generic;

namespace StorageService.Models
{
  public class BattleResult
  {
    public int FramesCount { get; set; }
    public IEnumerable<string> WinnersIds { get; set; }
  }
}