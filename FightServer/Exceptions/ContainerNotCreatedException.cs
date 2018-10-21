using System;

namespace FightServer.Exceptions
{
  internal sealed class ContainerNotCreatedException : Exception
  {
    public string DockerImage { get; }

    public ContainerNotCreatedException(string message, string dockerImage)
    {
      this.DockerImage = dockerImage;
    }

    public ContainerNotCreatedException(string message, string dockerImage, Exception innerException) : base(message, innerException)
    {
      this.DockerImage = dockerImage;
    }
  }
}