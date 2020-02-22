using GameLogic.Enums;
using GameLogic.Interfaces.Public;

namespace TestConsole.Models
{
	internal class UserAction : IAction
	{
		public UserActionType Type { get; set; }
		public Direction Direction { get; set;  }
	}
}