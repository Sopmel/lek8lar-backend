using Microsoft.AspNetCore.Mvc;
using Lek8LarBackend.Models;
using Lek8LarBackend.Services.MathGames.LevelOne;
using Lek8LarBackend.Models.MathGameModels.LevelOne;

namespace Lek8LarBackend.Controllers.MathGameControllers.LevelOne
{
    [ApiController]
    [Route("api/countgame")]
    public class CountGameController : ControllerBase
    {
        private static readonly Dictionary<Guid, CountGameSession> Sessions = new();
        private readonly CountGameService _service = new();

        [HttpPost("start")]
        public IActionResult StartGame([FromQuery] int level = 1)
        {
            var playerId = User.Identity?.Name ?? "guest";

            var session = new CountGameSession
            {
                PlayerId = playerId,
                Level = level
            };

            LevelOneCountGame? previousQuestion = null;

            for (int i = 0; i < session.TotalQuestions; i++)
            {
                LevelOneCountGame newQuestion;
                int attempts = 0;

                do
                {
                    newQuestion = _service.GenerateQuestion(level);
                    attempts++;
                }
                while (
                    previousQuestion is not null &&
                    newQuestion.ObjectImageUrl == previousQuestion.ObjectImageUrl &&
                    newQuestion.ObjectCount == previousQuestion.ObjectCount &&
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
        public IActionResult SubmitAnswer([FromBody] CountGameAnswerRequest request)
        {
            if (!Sessions.TryGetValue(request.SessionId, out var session)) return NotFound();

            var currentQ = session.Questions[session.CurrentQuestionNumber - 1];
            bool isCorrect = request.Answer == currentQ.CorrectAnswer;

            if (isCorrect)
                session.StarsEarned++;

            session.CurrentQuestionNumber++;

            if (session.CurrentQuestionNumber > session.TotalQuestions)
            {
                return Ok(new
                {
                    gameOver = true,
                    stars = session.StarsEarned,
                    level = session.Level, // 👈 nytt!
                    levelCleared = session.LevelCleared
                });
            }

            return Ok(new { correct = isCorrect });
        }


        public class CountGameAnswerRequest
        {
            public Guid SessionId { get; set; }
            public int Answer { get; set; }
        }
    }
}
