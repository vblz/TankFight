using System.Threading.Tasks;
using FightServer.Models;
using Refit;

namespace FightServer.HttpClients
{
	internal interface IStorageClient
	{
		[Post("/api/battle")]
		Task StartNewBattle(BattleInfo battleInfo);
		
		[Post("api/battle/{battleId}/frame")]
		Task AddFrame(string battleId, Frame frame);
	}
}