using Microsoft.AspNetCore.Mvc;
using Lek8LarBackend.Models.LetterHuntModels.LevelOne;
using Lek8LarBackend.Services.LetterHunt.LevelOne;

namespace Lek8LarBackend.Controllers.LetterHuntControllers.LevelOne
{
    [ApiController]
    [Route("api/letterhunt")]
    public class LetterHuntController : ControllerBase
    {
        private static readonly Dictionary<Guid, LetterHuntSession> Sessions = new();
        private readonly LetterHuntService _service = new();

        [HttpPost("start")]
        public IActionResult StartGame([FromQuery] int level = 1)
        {
            var playerId = User.Identity?.Name ?? "guest";

            var session = new LetterHuntSession
            {
                PlayerId = playerId,
                Level = level
            };

            LevelOneLetterHuntGame? previousQuestion = null;

            for (int i = 0; i < session.TotalQuestions; i++)
            {
                LevelOneLetterHuntGame newQuestion;
                int attempts = 0;

                do
                {
                    newQuestion = _service.GenerateQuestion(level);
                    attempts++;
                }
                while (
                    previousQuestion is not null &&
                    newQuestion.ImageUrl == previousQuestion.ImageUrl &&
                    newQuestion.CorrectLetter == previousQuestion.CorrectLetter &&
                    attempts < 10
                );

                session.Questions.Add(newQuestion);
                previousQuestion = newQuestion;
            }

            Sessions[session.SessionId] = session;

            return Ok(new { sessionId = session.SessionId });
        }

        [HttpGet("question")]
        public IActionResult GetQuestion([FromQuery] Guid sessionId)
        {
            if (!Sessions.TryGetValue(sessionId, out var session)) return NotFound();

            if (session.CurrentQuestionNumber > session.TotalQuestions)
            {
                return Ok(new
                {
                    gameOver = true,
                    stars = session.StarsEarned,
                    level = session.Level
                });
            }

            var question = session.Questions[session.CurrentQuestionNumber - 1];
            return Ok(question);
        }

        [HttpPost("answer")]
        public IActionResult SubmitAnswer([FromBody] LetterHuntAnswerRequest request)
        {
            if (!Sessions.TryGetValue(request.SessionId, out var session)) return NotFound();

            var currentQ = session.Questions[session.CurrentQuestionNumber - 1];
            bool isCorrect = request.Answer == currentQ.CorrectLetter;

            if (isCorrect)
                session.StarsEarned++;

            session.CurrentQuestionNumber++;

            if (session.CurrentQuestionNumber > session.TotalQuestions)
            {
                return Ok(new
                {
                    gameOver = true,
                    stars = session.StarsEarned,
                    correct = isCorrect,
                    level = session.Level,
                    levelCleared = session.LevelCleared
                });
            }

            return Ok(new { correct = isCorrect });
        }

        public class LetterHuntAnswerRequest
        {
            public Guid SessionId { get; set; }
            public string Answer { get; set; } = "";
        }
    }
}
