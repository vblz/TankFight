using System;

namespace StorageService.Exceptions
{
	internal sealed class BattleAlreadyExistsException : Exception
	{
		public BattleAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}