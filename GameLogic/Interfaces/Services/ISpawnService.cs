using System.Collections.Generic;
using GameLogic.Interfaces.Map;

namespace GameLogic.Interfaces.Services
{
	internal interface ISpawnService
	{
		void Spawn(IReadOnlyCollection<ICell> cells, IReadOnlyCollection<ICellContent> contents);
	}
}