using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StorageService.Exceptions;
using StorageService.Models;
using StorageService.Services.Interfaces;

namespace StorageService.Controllers
{
	[Produces("application/json")]
	[Route("api/battle/{battleId}/frame")]
	[ApiController]
	public sealed class FrameController : ControllerBase
	{
		private readonly IBattleStorage battleStorage;
		private readonly ILogger<BattleController> logger;

		[HttpGet("{frameNumber}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(500)]
		public async Task<ActionResult<Frame>> GetFrame([FromRoute] string battleId, [FromRoute] uint frameNumber)
		{
			if (string.IsNullOrEmpty(battleId))
			{
				return this.BadRequest(nameof(battleId));
			}

			try
			{
				return await this.battleStorage.GetFrame(battleId, frameNumber);
			}
			catch (BattleNotFoundException ex)
			{
				this.logger.LogWarning(ex, $"Ошибка получения кадра: боя {battleId} не существует, кадр {frameNumber}");
				return this.NotFound(nameof(battleId));
			}
			catch (FrameNotFoundException ex)
			{
				this.logger.LogWarning(ex, $"Ошибка получения кадра: бой {battleId} , кадра {frameNumber} не существует");
				return this.NotFound(nameof(frameNumber));
			}
			catch (Exception ex)
			{
				this.logger.LogWarning(ex, $"Ошибка получения кадра {battleId} {frameNumber}");
				return this.StatusCode(500);
			}
		}

		[HttpPost]
		[ProducesResponseType(201)]
		[ProducesResponseType(400)]
		[ProducesResponseType(409)]
		[ProducesResponseType(500)]
		public async Task<ActionResult> AddFrame([FromBody] Frame frame, [FromRoute] string battleId)
		{
			if (frame == null)
			{
				return this.BadRequest($"{nameof(frame)} не может быть пустым");
			}
			
			if (string.IsNullOrEmpty(frame.BattleId))
			{
				return this.BadRequest($"{nameof(frame.BattleId)} не может быть пустым");
			}
			
			if (battleId != frame.BattleId)
			{
				return this.BadRequest($"Значение {nameof(frame.BattleId)} не совпадает с запросом.");
			}

			if (frame.DestroyedInfo?.DestroyedBullets == null || frame.DestroyedInfo.DestroyedObjects == null)
			{
				return this.BadRequest($"{nameof(frame.DestroyedInfo)} не может быть пустым");
			}

			if (frame.GameState?.BulletsInfo == null || frame.GameState.ContentsInfo == null)
			{
				return this.BadRequest($"{nameof(frame.GameState)} не может быть пустым");
			}

			try
			{
				await this.battleStorage.PostFrame(frame);
			}
			catch (BattleNotFoundException ex)
			{
				this.logger.LogWarning(ex,
					$"Битва {frame.BattleId} не существует");
				return this.Conflict($"{nameof(frame.BattleId)} указан не верно");
			}
			catch (InvalidOperationException ex)
			{
				this.logger.LogWarning(ex,
					$"Фрейм с номером {frame.FrameNumber} не может быть добавлен в битву {frame.BattleId}");
				return this.Conflict($"{nameof(frame.FrameNumber)} указан не верно");
			}
			catch (BattleAlreadyFinishedException ex)
			{
				this.logger.LogWarning(ex,
					$"Фрейм с номером {frame.FrameNumber} не может быть добавлен в битву {frame.BattleId}");
				return this.Conflict($"Битва уже завершена");
			}
			catch (Exception ex)
			{
				this.logger.LogWarning(ex, $"Ошибка добавления фрейма {frame.FrameNumber} в битву {frame.BattleId}");
				return this.StatusCode(500);
			}

			return this.CreatedAtAction(nameof(this.GetFrame), new { frame.BattleId, frame.FrameNumber  }, null );
		}
		
		public FrameController(IBattleStorage battleStorage, ILogger<BattleController> logger)
		{
			this.battleStorage = battleStorage;
			this.logger = logger;
		}
	}
}