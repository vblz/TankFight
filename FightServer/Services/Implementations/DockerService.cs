using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using FightServer.Exceptions;
using FightServer.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FightServer.Services.Implementations
{
  public class DockerService : IDockerService
  {

    private readonly IDockerClient dockerClient;
    private readonly ILogger<DockerService> logger;
    private readonly TimeSpan TimeForContainerResponse = TimeSpan.FromSeconds(10);

    public async Task<string> CreateAndStartContainer(string imageName)
    {
      //Chirkov_IA Не нашел, как ограничить размер, думаю надо ограничивать до того, как образ попал сюда.
      var containerId = await this.CreateContainer(imageName);
      await this.StartContainer(containerId);

      return containerId;
    }

    public async Task StopContainer(string containerId)
    {
      try
      {
        // Chirkov_IA не проверяется на true, т.к. false может быть, если такой контейнер уже остановлен. Иначе - эксепшен.
        await this.dockerClient.Containers.StopContainerAsync(containerId, new ContainerStopParameters());
      }
      catch (DockerContainerNotFoundException ex)
      {
        this.logger.LogWarning(ex, $"Не найден контейнер {containerId}");
      }
      catch (Exception ex)
      {
        this.logger.LogError(ex, $"Ошибка при остановке контейнера {containerId}");
        throw;
      }
    }

    public async Task<string> AskContainer(string containerId, string stdIn)
    {
      var attachStream = await this.dockerClient.Containers.AttachContainerAsync(containerId, false,
        new ContainerAttachParameters
        {
          Stream = true,
          Stdin = true,
          Stdout = true,
          Stderr = true
        });

      // FIXME Не успел проверить. На моем образе не работало
      await attachStream.WriteAsync(Encoding.ASCII.GetBytes(stdIn), 0, stdIn.Length, CancellationToken.None);

      var cts = new CancellationTokenSource(TimeForContainerResponse);

      //FIXME Не успел проверить. На моем образе не работало
      var (stdout, stderr) = await attachStream.ReadOutputToEndAsync(cts.Token);

      if (string.IsNullOrWhiteSpace(stdout))
      {
        throw new ContainerAnswerException("Контейнер не ответил за установленное время или ответил пустой строкой.");
      }

      this.logger.LogWarning($"Контейнер вернул stderr: {stderr}");

      return stdout;
    }

    private async Task<string> CreateContainer(string imageName)
    {
      try
      {
        var createResult = await this.dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
        {
          AttachStdin = true,
          AttachStdout = true,
          AttachStderr = true,
          NetworkDisabled = true,
          Image = imageName
          //TODO Возможно, сюда добавить User, от кого будут выполняться команды внутри контейнера.
        });

        return createResult.ID;
      }
      catch (Exception ex)
      {
        this.logger.LogError(ex, $"Не удалось создать контейнер для образа {imageName}.");
        throw new ContainerNotCreatedException("Не удалось создать контейнер.", imageName, ex);
      }
    }

    private async Task StartContainer(string containerId)
    {
      try
      {
        // Chirkov_IA не проверяется на true, т.к. false может быть, если такой контейнер уже запущен. Иначе - эксепшен.
        await this.dockerClient.Containers.StartContainerAsync(containerId, new ContainerStartParameters());
      }
      catch (Exception ex)
      {
        this.logger.LogError(ex, $"Не удалось запустить контейнер {containerId}.");
        throw new ContainerNotStartedException("Не удалось запустить контейнер.", containerId, ex);
      }
    }

    public DockerService(IDockerClient dockerClient, ILogger<DockerService> logger)
    {
      this.dockerClient = dockerClient;
      this.logger = logger;
    }
  }
}