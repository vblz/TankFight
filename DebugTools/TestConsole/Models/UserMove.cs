using System.Collections.Generic;
using GameLogic.Interfaces.Public;

namespace TestConsole.Models
{
	internal sealed class UserMove : IUserMove
	{
		public string UserId => "1";
		public IReadOnlyList<IAction> Actions { get; }
		
		public UserMove(IReadOnlyList<IAction> actions)
		{
			this.Actions = actions;
		}
	}
}