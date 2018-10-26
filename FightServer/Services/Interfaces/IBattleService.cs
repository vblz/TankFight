using System.Collections.Generic;
using FightServer.Models;

namespace FightServer.Services.Interfaces
{
  public interface IBattleService
  {
    BattleInfo StartNew(ISet<string> dockerImages);
  }
}