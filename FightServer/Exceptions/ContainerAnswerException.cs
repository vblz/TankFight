using System;

namespace FightServer.Exceptions
{
  public class ContainerAnswerException : Exception
  {
    public ContainerAnswerException(string message) : base(message)
    {
    }

    public ContainerAnswerException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}