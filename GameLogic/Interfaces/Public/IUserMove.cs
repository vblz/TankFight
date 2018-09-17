using System.Collections.Generic;

namespace GameLogic.Interfaces.Public
{
	/// <summary>
	/// Не в смысле движение, но в смысле ход.
	/// </summary>
	public interface IUserMove
	{
		string UserId { get; }
		IReadOnlyList<IAction> Actions { get; }
	}
}