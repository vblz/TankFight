using System.Collections.Generic;

namespace GameLogic.Interfaces.Public
{
	public interface IMapInfo
	{
		IReadOnlyCollection<ICellContentInfo> MapObjects { get; }
		byte Height { get; }
		byte Width { get; }
	}
}