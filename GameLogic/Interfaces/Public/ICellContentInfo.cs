using GameLogic.Enums;
using GameLogic.Implementations.Public;

namespace GameLogic.Interfaces.Public
{
	public interface ICellContentInfo
	{
		Coordinates Coordinates { get; }
		byte HealthCount { get; }
		CellContentType Type { get; }
		// FIXME костыль, убрать
		string UserId { get; set; }
	}
}