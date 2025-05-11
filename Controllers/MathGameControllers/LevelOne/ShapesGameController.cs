using Lek8LarBackend.Models.MathGameModels.LevelOne;
using Lek8LarBackend.Services.MathGames.LevelOne;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Lek8LarBackend.Controllers.MathGames.LevelOne
{
    [ApiController]
    [Route("api/shapesgame")]
    public class ShapeGameController : ControllerBase
    {
        private static Dictionary<string, ShapeGameSession> sessions = new();
        private readonly ShapeGameService _service;

        public ShapeGameController(ShapeGameService service)
        {
            _service = service;
        }

        [HttpPost("start")]
        public IActionResult StartGame([FromQuery] int level = 1)
        {
            var session = new ShapeGameSession();
            sessions[session.SessionId.ToString()] = session;

            return Ok(new { sessionId = session.SessionId, level });
        }

        [HttpGet("question")]
        public IActionResult GetQuestion([FromQuery] string sessionId, [FromQuery] int level = 1)
        {
            if (!sessions.TryGetValue(sessionId, out var session))
                return BadRequest("Ogiltigt sessions-ID");

            if (session.CurrentQuestionNumber > session.TotalQuestions)
                return Ok(new { gameOver = true });

            LevelOneShapeGame newQuestion;
            int attempts = 0;
            LevelOneShapeGame? previous = session.Questions.LastOrDefault();

            do
            {
                newQuestion = _service.GenerateQuestion(session.SessionId, level);
                attempts++;
            }
            while (
                previous is not null &&
                newQuestion.ShapeImageUrl == previous.ShapeImageUrl &&
                newQuestion.CorrectAnswer == previous.CorrectAnswer &&
                attempts < 10
            );

            newQuestion.SessionId = Guid.Parse(sessionId);
            session.Questions.Add(newQuestion);
            session.CurrentQuestionNumber++;

            return Ok(newQuestion);
        }

        [HttpPost("answer")]
        public IActionResult SubmitAnswer([FromBody] ShapeGameAnswer answer)
        {
            if (!sessions.TryGetValue(answer.SessionId, out var session))
                return BadRequest("Ogiltigt sessions-ID");

            var isCorrect = _service.CheckAnswer(answer.Answer, Guid.Parse(answer.SessionId));

            if (isCorrect)
            {
                session.StarsEarned++;
            }

            return Ok(new
            {
                correct = isCorrect,
                gameOver = session.CurrentQuestionNumber > session.TotalQuestions,
                stars = session.StarsEarned,
                levelCleared = session.StarsEarned == session.TotalQuestions
            });
        }
    }

    public class ShapeGameAnswer
    {
        public int QuestionId { get; set; }
        public string Answer { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
    }
}
