using System.Collections.Generic;
using GameLogic.Interfaces.Public;

namespace FightServer.Models
{
	internal sealed class UserMove : IUserMove
	{
		public string UserId { get; }
		public IReadOnlyList<IAction> Actions { get; }
		
		public UserMove(IReadOnlyList<IAction> actions, string userId)
		{
			this.Actions = actions;
			this.UserId = userId;
		}
	}
}