using System;
using System.Runtime.Serialization;

namespace GameLogic.Implementations.Exceptions
{
	public sealed class UserNotFoundException : Exception
	{
		public UserNotFoundException()
		{
		}

		public UserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public UserNotFoundException(string message) : base(message)
		{
		}

		public UserNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}