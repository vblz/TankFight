using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FightServer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FightServer.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class FightController : ControllerBase
  {
    private readonly IBattleService battleService;

    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> StartNew([FromBody] string[] dockerImages)
    {
      if (dockerImages == null || dockerImages.Length == 0)
      {
        return this.BadRequest();
      }

      var dockerImagesSet = new HashSet<string>(dockerImages);

      if (dockerImagesSet.Count != dockerImages.Length)
      {
        return this.BadRequest();
      }

      if (dockerImagesSet.Any(string.IsNullOrEmpty))
      {
        return this.BadRequest();
      }

      var battleInfo = await this.battleService.StartNew(dockerImagesSet);

      return this.Ok(battleInfo);
    }

    public FightController(IBattleService battleService)
    {
      this.battleService = battleService;
    }
  }
}