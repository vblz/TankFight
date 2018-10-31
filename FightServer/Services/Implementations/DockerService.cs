using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using FightServer.Exceptions;
using FightServer.Services.Interfaces;
using FightServer.Settings;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FightServer.Services.Implementations
{
  public class DockerService : IDockerService
  {
    private static readonly TimeSpan timeForContainerResponse = TimeSpan.FromSeconds(5);
    private static readonly byte NewLineChar = (byte)'\n'; // ASCII

    private readonly IDockerClient dockerClient;
    private readonly ILogger<DockerService> logger;
    private readonly ContainerSettings settings;

    public async Task<string> CreateAndStartContainer(string imageName)
    {
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
      using (var attachStream = await this.dockerClient.Containers.AttachContainerAsync(containerId, false,
        new ContainerAttachParameters
        {
          Stream = true,
          Stdin = true,
          Stdout = true
        }))
      {
        var writeBuffer = Encoding.UTF8.GetBytes(stdIn);
        await attachStream.WriteAsync(writeBuffer, 0, writeBuffer.Length, CancellationToken.None);

        var cts = new CancellationTokenSource(timeForContainerResponse);

        var stdOutTask = this.ReadLineAsync(attachStream, cts.Token);
        await Task.WhenAny(stdOutTask, Task.Delay(timeForContainerResponse));

        if (!stdOutTask.IsCompletedSuccessfully)
        {
          cts.Cancel();
          throw new ContainerAnswerException("Контейнер не ответил");
        }

        var result = await stdOutTask;
        if (string.IsNullOrEmpty(result))
        {
          throw new ContainerAnswerException("Контейнер ответил пустой строкой");
        }

        return result;
      }
    }

    public async Task<string> ReadLineAsync(MultiplexedStream multiplexedStream, CancellationToken cancellationToken)
    {
      List<byte> received = new List<byte>();
      byte[] buffer = new byte[15];

      while (!cancellationToken.IsCancellationRequested)
      {
        var readResult =
          await multiplexedStream.ReadOutputAsync(buffer, 0, buffer.Length, cancellationToken);

        if (readResult.Target != MultiplexedStream.TargetStream.StandardOut)
        {
          continue;
        }

        int newLineIndex = -1;

        for (int i = 0; i < readResult.Count; ++i)
        {
          if (buffer[i] == NewLineChar)
          {
            newLineIndex = i;
            break;
          }
        }

        if (newLineIndex != -1)
        {
          received.AddRange(buffer.Take(newLineIndex));
          break;
        }

        received.AddRange(buffer.Take(readResult.Count));
      }

      return Encoding.UTF8.GetString(received.ToArray());
    }

    private async Task<string> CreateContainer(string imageName)
    {
      try
      {
        var createResult = await this.dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
        {
          AttachStdin = true,
          AttachStdout = true,
          NetworkDisabled = true,
          Tty = false,
          OpenStdin = true,
          Image = imageName,
          StopTimeout = TimeSpan.Zero,
          HostConfig = new HostConfig
          {
            Memory = 512 * 1024 * 1024,
            MemorySwap = 0,
            CPUCount = 1,
            AutoRemove = !this.settings.DontRemoveContainers,
            BlkioDeviceWriteBps = new List<ThrottleDevice>()
            {
              new ThrottleDevice
              {
                Path = "/dev/sda",
                Rate = 1024 * 1024
              }
            }
          }
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

    public DockerService(IDockerClient dockerClient, ILogger<DockerService> logger, IOptions<ContainerSettings> settings)
    {
      this.dockerClient = dockerClient;
      this.logger = logger;
      this.settings = settings.Value ?? throw new ArgumentNullException();
    }
  }
}