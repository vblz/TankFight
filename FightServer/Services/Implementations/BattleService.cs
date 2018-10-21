using System;
using System.Threading;
using System.Threading.Tasks;
using FightServer.HttpClients;
using FightServer.Models;
using FightServer.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Refit;

namespace FightServer.Services.Implementations
{
	internal sealed class BattleService : IBattleService
	{
		private readonly IDockerService dockerService;
		private readonly ILogger<BattleService> logger;
		private readonly ILoggerFactory loggerFactory;

		public BattleInfo StartNew(string[] dockerImages)
		{
			var battleInfo = new BattleInfo
			{
				BattleId = Guid.NewGuid().ToString(),
				Map = @"D:/"
			};
			
			var battle = new Battle(battleInfo, dockerImages, this.dockerService, this.CreateStorageClient(),
				this.loggerFactory.CreateLogger<Battle>());
			
#pragma warning disable 4014
			battle.Start(new CancellationToken());
#pragma warning restore 4014

			return battleInfo;
		}

		private IStorageClient CreateStorageClient() => RestService.For<IStorageClient>("http://localhost:5005");

		public BattleService(IDockerService dockerService, ILogger<BattleService> logger, ILoggerFactory loggerFactory)
		{
			this.dockerService = dockerService;
			this.logger = logger;
			this.loggerFactory = loggerFactory;
		}
	}
}