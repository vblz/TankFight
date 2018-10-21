using System;
using System.Net;
using System.Threading.Tasks;
using ImageService.Models;
using ImageService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ImageService.Controllers
{
  [Produces("application/json")]
  [Route("api/[controller]")]
  [ApiController]
  public class CreateController : Controller
  {
    //Не уверен, что пригодится, добавил для теста
    private string Test =
      "using System; using System.Linq; using System.IO; using System.Text; using System.Collections; using System.Collections.Generic; using System.Net.Http; public class Player{    static void Main(string[] args)    {        string inputs;        while (true)        {            inputs = Console.ReadLine();            if(!string.IsNullOrWhiteSpace(inputs))            {               Console.WriteLine(new { Type = new Random().Next(1), Direction = new Random().Next(3) });            }        }    }}";

    private readonly IImageCreator imageCreator;
    private readonly ILogger<CreateController> logger;

    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Create(SupportedLanguages language, [FromBody] string code)
    {
      if (string.IsNullOrWhiteSpace(code))
      {
        return this.BadRequest();
      }

      try
      {
        var imageTag = await imageCreator.CreateImage(language, Test);

        return this.Ok(new { Tag = imageTag });
      }
      catch (Exception ex)
      {
        this.logger.LogWarning(ex, "Не удалось создать образ из кода.");
        return this.StatusCode((int)HttpStatusCode.InternalServerError);
      }
    }

    public CreateController(IImageCreator imageCreator, ILogger<CreateController> logger)
    {
      this.imageCreator = imageCreator;
      this.logger = logger;
    }
  }
}