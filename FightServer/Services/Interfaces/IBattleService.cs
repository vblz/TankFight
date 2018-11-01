using System.Collections.Generic;
using System.Threading.Tasks;
using FightServer.Models;

namespace FightServer.Services.Interfaces
{
  public interface IBattleService
  {
    Task<BattleInfo> StartNew(ISet<string> dockerImages);
  }
}