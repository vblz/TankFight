using System.Collections.Generic;
using System.Threading.Tasks;
using StorageService.Models;

namespace StorageService.Services.Interfaces
{
	public interface IBattleStorage
	{
		Task<Frame> GetFrame(string battleId, uint frameNumber);
		Task<IEnumerable<string>> GetWinners(string battleId);
		Task PostFrame(Frame frame);
		Task StartNewBattle(BattleInfo battleInfo);
	}
}