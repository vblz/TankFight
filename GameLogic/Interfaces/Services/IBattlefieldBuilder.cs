using GameLogic.Interfaces.Map;
using GameLogic.Interfaces.Public;

namespace GameLogic.Interfaces.Services
{
	internal interface IBattlefieldBuilder
	{
		IBattlefield Build(IMapInfo mapInfo);
	}
}