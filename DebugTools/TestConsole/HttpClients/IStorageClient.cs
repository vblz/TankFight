using System.Net.Http;
using System.Threading.Tasks;
using Refit;
using TestConsole.Models.StorageService;

namespace TestConsole.HttpClients
{
	internal interface IStorageClient
	{
		[Post("/api/battle")]
		Task StartNewBattle(BattleInfo battleInfo);
		
		[Post("/api/battle/{battleId}/frame")]
		Task<HttpResponseMessage> AddFrame(Frame frame, string battleId);
	}
}