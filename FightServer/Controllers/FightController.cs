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
	  public IActionResult StartNew([FromBody] string[] dockerImages)
	  {
	    if (dockerImages == null || dockerImages.Length == 0)
	    {
	      return this.BadRequest();
	    }

	    var battleInfo = this.battleService.StartNew(dockerImages);

	    return this.Ok(battleInfo);
	  }

    public FightController(IBattleService battleService)
	  {
	    this.battleService = battleService;
	  }
	}
}