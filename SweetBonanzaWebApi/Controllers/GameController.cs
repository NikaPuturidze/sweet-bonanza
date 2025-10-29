using Microsoft.AspNetCore.Mvc;
using SweetBonanza.WebApi.Services;

namespace SweetBonanza.WebApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class GameController(GameService _gameService) : ControllerBase
    {
        [HttpGet("spin")]
        public IActionResult Spin()
        {
            var result = _gameService.RunGame();
            return Ok(result);
        }
    }
}
