namespace Lek8LarBackend.Controllers.MathGameControllers.LevelOne
{
    using Microsoft.AspNetCore.Mvc;
    using Lek8LarBackend.Models;
    using Lek8LarBackend.Services;
    using Lek8LarBackend.Models.MathGameModels.LevelOne;
    using Lek8LarBackend.Services.MathGames.LevelOne;

    [ApiController]
    [Route("api/[controller]")]
    public class CountGameController : ControllerBase
    {
        private static readonly Dictionary<Guid, CountGameSession> Sessions = new();
        private readonly CountGameService _service = new();

        [HttpPost("start")]
        public IActionResult StartGame([FromQuery] string difficulty = "easy")
        {
            var playerId = User.Identity?.Name ?? "guest";

            var session = new CountGameSession
            {
                PlayerId = playerId
            };

            for (int i = 0; i < session.TotalQuestions; i++)
            {
                session.Questions.Add(_service.GenerateQuestion(difficulty));
            }

            Sessions[session.SessionId] = session;

            return Ok(new { sessionId = session.SessionId });
        }

        [HttpGet("question")]
        public IActionResult GetQuestion([FromQuery] Guid sessionId)
        {
            if (!Sessions.TryGetValue(sessionId, out var session)) return NotFound();

            if (session.CurrentQuestionNumber > session.TotalQuestions)
                return Ok(new { gameOver = true, stars = session.StarsEarned });

            var question = session.Questions[session.CurrentQuestionNumber - 1];
            return Ok(question);
        }

        [HttpPost("answer")]
        public IActionResult SubmitAnswer([FromQuery] Guid sessionId, [FromBody] int answer)
        {
            if (!Sessions.TryGetValue(sessionId, out var session)) return NotFound();

            var currentQ = session.Questions[session.CurrentQuestionNumber - 1];

            if (answer == currentQ.CorrectAnswer)
                session.StarsEarned += 1;

            session.CurrentQuestionNumber++;

            if (session.CurrentQuestionNumber > session.TotalQuestions)
            {
                return Ok(new
                {
                    gameOver = true,
                    stars = session.StarsEarned,
                    levelCleared = session.LevelCleared
                });
            }

            return Ok(new { correct = answer == currentQ.CorrectAnswer });
        }
    }

}
