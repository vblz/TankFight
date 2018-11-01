using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FightServer.HttpClients;
using FightServer.Models;
using FightServer.Services.Interfaces;
using FightServer.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;

namespace FightServer.Services.Implementations
{
  internal sealed class BattleService : IBattleService
  {
    private readonly IDockerService dockerService;
    private readonly ILogger<BattleService> logger;
    private readonly ILoggerFactory loggerFactory;
    private readonly BattleSettings battleSettings;

    public Task<BattleInfo> StartNew(ISet<string> dockerImages)
    {
      var battle = new Battle(battleSettings, dockerImages, this.dockerService, this.CreateStorageClient(),
        this.loggerFactory.CreateLogger<Battle>());

      return battle.Start(new CancellationToken());
    }

    private IStorageClient CreateStorageClient() => RestService.For<IStorageClient>(this.battleSettings.StorageServiceLocation);

    public BattleService(IDockerService dockerService, ILogger<BattleService> logger, ILoggerFactory loggerFactory, IOptions<BattleSettings> battleSettings)
    {
      this.dockerService = dockerService;
      this.logger = logger;
      this.loggerFactory = loggerFactory;
      this.battleSettings = battleSettings.Value ?? throw new ArgumentNullException();
    }
  }
}