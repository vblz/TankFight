using System;
using System.Threading.Tasks;

namespace FightServer.Services.Interfaces
{
	public interface IDockerService
	{
		// проверять размер
		// ВЫНЕСТИ В ДРУГОЙ СЕРВИС
		//Task GetImages(string[] dockerHubImages);
		
		// ограничить размер и сеть
		Task<string> CreateAndStartContainer(string imageName);
		
		Task StopContainer(string containerId);

		// не забыть про таймаут
		Task<string> AskContainer(string containerId, string stdIn, TimeSpan maxAnswerTime);
	}
}