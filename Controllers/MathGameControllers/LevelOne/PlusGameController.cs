using Microsoft.AspNetCore.Mvc;
using Lek8LarBackend.Models;
using Lek8LarBackend.Services.MathGames.LevelOne;
using Lek8LarBackend.Models.MathGameModels.LevelOne;
using System.Collections.Concurrent;

namespace Lek8LarBackend.Controllers.MathGameControllers.LevelOne
{
    [ApiController]
    [Route("api/plusgame")]
    public class PlusGameController : ControllerBase
    {
        private static readonly ConcurrentDictionary<Guid, PlusGameSession> Sessions = new();
        private readonly PlusGameService _service = new();

        [HttpPost("start")]
        public IActionResult StartGame([FromQuery] int level = 1)
        {
            var playerId = User.Identity?.Name ?? "guest";

            var session = new PlusGameSession
            {
                PlayerId = playerId,
                Level = level
            };

            LevelOnePlusGame? previousQuestion = null;

            for (int i = 0; i < session.TotalQuestions; i++)
            {
                LevelOnePlusGame newQuestion;
                int attempts = 0;

                do
                {
                    newQuestion = _service.GenerateQuestion(level);
                    attempts++;
                }
                while (
                    previousQuestion is not null &&
                    newQuestion.ObjectImageUrl == previousQuestion.ObjectImageUrl &&
                    newQuestion.GroupA == previousQuestion.GroupA &&
                    newQuestion.GroupB == previousQuestion.GroupB &&
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
            if (!Sessions.TryGetValue(sessionId, out var session))
                return NotFound();

            if (session.CurrentQuestionNumber > session.TotalQuestions)
            {
                var finalResult = new GameResult
                {
                    GameOver = true,
                    Stars = session.StarsEarned,
                    Level = session.Level,
                    LevelCleared = true
                };

                return Ok(new LevelOnePlusGame
                {
                    Id = -1,
                    ObjectImageUrl = "gameover.png",
                    GroupA = 0,
                    GroupB = 0,
                    Options = new(),
                    GameResult = finalResult
                });
            }

            var question = session.Questions[session.CurrentQuestionNumber - 1];
            question.GameResult = new GameResult
            {
                GameOver = false,
                Stars = session.StarsEarned,
                Level = session.Level,
                LevelCleared = false
            };

            return Ok(question);
        }

        [HttpPost("answer")]
        public IActionResult SubmitAnswer([FromBody] PlusGameAnswerRequest request)
        {
            if (!Sessions.TryGetValue(request.SessionId, out var session))
                return NotFound();

            var currentIndex = session.CurrentQuestionNumber - 1;
            var question = session.Questions[currentIndex];

            bool isCorrect = request.Answer == question.CorrectAnswer;
            if (isCorrect)
                session.StarsEarned++;

            session.CurrentQuestionNumber++;

            var isGameOver = session.CurrentQuestionNumber > session.TotalQuestions;

            question.GameResult = new GameResult
            {
                GameOver = isGameOver,
                Stars = session.StarsEarned,
                Level = session.Level,
                LevelCleared = isGameOver
            };

            return Ok(question);
        }

        public class PlusGameAnswerRequest
        {
            public Guid SessionId { get; set; }
            public int Answer { get; set; }
        }
    }
}
