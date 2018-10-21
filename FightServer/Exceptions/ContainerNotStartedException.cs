using System;

namespace FightServer.Exceptions
{
  internal sealed class ContainerNotStartedException : Exception
  {
    public string ContainerId { get; }

    public ContainerNotStartedException(string message, string containerId)
    {
      this.ContainerId = containerId;
    }

    public ContainerNotStartedException(string message, string containerId, Exception innerException) : base(message, innerException)
    {
      this.ContainerId = containerId;
    }
  }
}