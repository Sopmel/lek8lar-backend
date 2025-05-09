using Lek8LarBackend.Models.MathGameModels.LevelOne;
using Lek8LarBackend.Services.MathGames.LevelOne;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Lek8LarBackend.Controllers.MathGames.LevelOne
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShapeGameController : ControllerBase
    {
        private static Dictionary<string, ShapeGameSession> sessions = new();
        private readonly ShapeGameService _service;

        public ShapeGameController(ShapeGameService service)
        {
            _service = service;
        }

        [HttpPost("start")]
        public IActionResult StartGame()
        {
            var session = new ShapeGameSession();
            sessions[session.SessionId.ToString()] = session;

            return Ok(new { sessionId = session.SessionId });
        }

        [HttpGet("question")]
        public IActionResult GetQuestion([FromQuery] string sessionId)
        {
            if (!sessions.TryGetValue(sessionId, out var session))
                return BadRequest("Ogiltigt sessions-ID");

            if (session.CurrentQuestionNumber > session.TotalQuestions)
                return Ok(new { gameOver = true });

            var question = _service.GenerateQuestion(session.SessionId);
            question.SessionId = Guid.Parse(sessionId);
            session.Questions.Add(question);
            session.CurrentQuestionNumber++;

            return Ok(question);
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
