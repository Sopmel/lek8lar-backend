using Microsoft.AspNetCore.Mvc;
using Lek8LarBackend.Models.WordMatchModels.LevelOne;
using Lek8LarBackend.Services.WordMatch.LevelOne;

namespace Lek8LarBackend.Controllers.WordMatchControllers.LevelOne
{
    [ApiController]
    [Route("api/wordmatch")]
    public class WordMatchController : ControllerBase
    {
        private static readonly Dictionary<Guid, WordMatchSession> Sessions = new();
        private readonly WordMatchService _service = new();

        [HttpPost("start")]
        public IActionResult StartGame([FromQuery] int level = 1)
        {
            var playerId = User.Identity?.Name ?? "guest";

            var session = new WordMatchSession
            {
                PlayerId = playerId,
                Level = level
            };

            LevelOneWordMatchGame? previous = null;

            for (int i = 0; i < session.TotalQuestions; i++)
            {
                LevelOneWordMatchGame question;
                int attempts = 0;

                do
                {
                    question = _service.GenerateQuestion(level);
                    attempts++;
                }
                while (previous is not null && question.ImageUrl == previous.ImageUrl && attempts < 10);

                session.Questions.Add(question);
                previous = question;
            }

            Sessions[session.SessionId] = session;

            return Ok(new { sessionId = session.SessionId });
        }

        [HttpGet("question")]
        public IActionResult GetQuestion([FromQuery] Guid sessionId)
        {
            if (!Sessions.TryGetValue(sessionId, out var session))
                return NotFound();

            if (session.CurrentQuestionNumber > session.TotalQuestions)
            {
                return Ok(new
                {
                    gameOver = true,
                    stars = session.StarsEarned,
                    level = session.Level
                });
            }

            return Ok(session.Questions[session.CurrentQuestionNumber - 1]);
        }

        [HttpPost("answer")]
        public IActionResult SubmitAnswer([FromBody] WordMatchAnswerRequest request)
        {
            if (!Sessions.TryGetValue(request.SessionId, out var session))
                return NotFound();

            var current = session.Questions[session.CurrentQuestionNumber - 1];
            bool isCorrect = request.Answer == current.CorrectImageName;

            if (isCorrect)
                session.StarsEarned++;

            session.CurrentQuestionNumber++;

            if (session.CurrentQuestionNumber > session.TotalQuestions)
            {
                return Ok(new
                {
                    gameOver = true,
                    correct = isCorrect,
                    stars = session.StarsEarned,
                    level = session.Level,
                    levelCleared = session.LevelCleared
                });
            }

            return Ok(new
            {
                correct = isCorrect,
                stars = session.StarsEarned,
                gameOver = false
            });
        }

        public class WordMatchAnswerRequest
        {
            public Guid SessionId { get; set; }
            public string Answer { get; set; } = "";
        }
    }
}
