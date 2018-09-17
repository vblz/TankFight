using GameLogic.Implementations.Public;

namespace GameLogic.Interfaces.Map
{
	internal interface ICell
	{
		ICellContent Content { get; }
		Coordinates Coordinates { get; }
		bool IsEmpty { get; }
		
		void Put(ICellContent content);
		ICellContent Pop();
	}
}