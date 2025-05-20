using Lek8LarBackend.Services.MemoryGames;
using Microsoft.AspNetCore.Mvc;

namespace Lek8LarBackend.Controllers.MemoryGameControllers.LevelOne
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColorMixGameController : ControllerBase
    {
        private readonly ColorMixGameService _gameService;

        public ColorMixGameController(ColorMixGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("start")]
        public IActionResult StartGame([FromQuery] int level)
        {
            var session = _gameService.StartNewSession(level);
            return Ok(new { sessionId = session.SessionId });
        }

        [HttpGet("question")]
        public IActionResult GetQuestion([FromQuery] string sessionId)
        {
            var question = _gameService.GetNextQuestion(sessionId);
            if (question == null)
                return NotFound();

            return Ok(question);
        }

        [HttpPost("answer")]
        public IActionResult SubmitAnswer([FromBody] AnswerRequest request)
        {
            var (correct, gameOver, stars) = _gameService.SubmitAnswer(request.SessionId, request.Answer);
            return Ok(new { correct, gameOver, stars });
        }

        [HttpPost("progress")]
        public IActionResult SubmitProgress([FromBody] GameResult result)
        {
            _gameService.EndGame(result.SessionId, result.Stars);
            return Ok();
        }
    }

    public class AnswerRequest
    {
        public string SessionId { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
    }

    public class GameResult
    {
        public string SessionId { get; set; } = string.Empty;
        public int Stars { get; set; }
        public int Level { get; set; }
        public bool GameOver { get; set; }
        public bool LevelCleared { get; set; }
    }
}