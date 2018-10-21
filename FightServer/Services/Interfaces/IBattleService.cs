using System.Threading.Tasks;
using FightServer.Models;

namespace FightServer.Services.Interfaces
{
	public interface IBattleService
	{
		BattleInfo StartNew(string[] dockerImages);
	}
}