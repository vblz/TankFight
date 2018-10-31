using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StorageService.Exceptions;
using StorageService.Models;
using StorageService.Services.Interfaces;

namespace StorageService.Controllers
{
	[Produces("application/json")]
	[Route("api/battle")]
	[ApiController]
	public class BattleController : ControllerBase
	{
		private readonly IBattleStorage battleStorage;
		private readonly ILogger<BattleController> logger;

		[HttpPost]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(500)]
		public async Task<ActionResult> StartNewBattle([FromBody] BattleInfo battleInfo)
		{

			if (battleInfo == null
			    || string.IsNullOrEmpty(battleInfo.BattleId)
			    || string.IsNullOrEmpty(battleInfo.Map))
			{
				return this.BadRequest();
			}

			try
			{
				await this.battleStorage.StartNewBattle(battleInfo);
				// TODO: может быть, стоит сделать получение информации о битве по battleId - карты, кол-ва фрейом, участников,
				// победителей. Тогда сделать тут правильно: return this.Created()
				return this.Ok();
			}
			catch (BattleAlreadyExistsException ex)
			{
				this.logger.LogWarning(ex, $"Попытка стартовать бой {battleInfo.BattleId} повторно");
				return this.BadRequest(nameof(battleInfo.BattleId));
			}
			catch (Exception ex)
			{
				this.logger.LogWarning(ex, $"Ошибка начала боя {battleInfo.BattleId}");
				return this.StatusCode(500);
			}
		}

		[HttpGet("{battleId}/")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<ActionResult<BattleInfo>> GetBattleInfo([FromRoute] string battleId)
		{
			if (string.IsNullOrEmpty(battleId))
			{
				return this.BadRequest(nameof(battleId));
			}

			try
			{
				return new ActionResult<BattleInfo>(await this.battleStorage.GetBattle(battleId));
			}
			catch (BattleNotFoundException ex)
			{
				this.logger.LogWarning(ex, $"Ошибка получения информации: бой {battleId} не найден");
				return this.NotFound();
			}
			catch (Exception ex)
			{
				this.logger.LogWarning(ex, $"Ошибка получения информации {battleId}");
				return this.StatusCode(500);
			}
		}

		[HttpGet("{battleId}/winners")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(409)]
		[ProducesResponseType(500)]
		public async Task<ActionResult<BattleResult>> GetWinners([FromRoute] string battleId)
		{
			if (string.IsNullOrEmpty(battleId))
			{
				return this.BadRequest(nameof(battleId));
			}

			try
			{
				return new ActionResult<BattleResult>(await this.battleStorage.GetWinners(battleId));
			}
			catch (BattleNotFoundException ex)
			{
				this.logger.LogWarning(ex, $"Ошибка получения победителей: бой {battleId} не найден");
				return this.NotFound();
			}
			catch (BattleNotFinishedException ex)
			{
				this.logger.LogWarning(ex, $"Ошибка получения победителей: бой {battleId} не закончился");
				return this.Conflict();
			}
			catch (Exception ex)
			{
				this.logger.LogWarning(ex, $"Ошибка получения победителей {battleId}");
				return this.StatusCode(500);
			}
		}

		public BattleController(IBattleStorage battleStorage, ILogger<BattleController> logger)
		{
			this.battleStorage = battleStorage;
			this.logger = logger;
		}
	}
}