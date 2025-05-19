using Lek8LarBackend.Data.Entities;
using Lek8LarBackend.Data;
using Lek8LarBackend.Models.MemoryGameModels;
using Microsoft.AspNetCore.Mvc;
using Lek8LarBackend.Services.MemoryGames;

namespace Lek8LarBackend.Controllers.MemoryGameControllers.LevelOne
{
    [ApiController]
    [Route("api/whatsmissing")]
    public class WhatsMissingController : ControllerBase
    {
        private static readonly Dictionary<Guid, WhatsMissingSession> Sessions = new();
        private readonly ApplicationDbContext _db;
        private readonly WhatsMissingService _whatsMissingService;

        public WhatsMissingController(ApplicationDbContext db, WhatsMissingService whatsMissingService)
        {
            _db = db;
            _whatsMissingService = whatsMissingService;
        }

        [HttpPost("start")]
        public IActionResult StartGame()
        {
            var session = new WhatsMissingSession
            {
                PlayerId = User.Identity?.Name ?? "guest",
                Level = 1
            };

            Sessions[session.SessionId] = session;
            return Ok(new { sessionId = session.SessionId });
        }

        [HttpGet("question")]
        public IActionResult GetQuestion([FromQuery] Guid sessionId)
        {
            if (!Sessions.TryGetValue(sessionId, out var session)) return NotFound();

            if (session.Stars >= 5)
            {
                return Ok(new GameResult
                {
                    GameOver = true,
                    Stars = session.Stars,
                    Level = session.Level,
                    LevelCleared = true
                });
            }

            var question = _whatsMissingService.GenerateQuestion();
            session.Questions.Add(question);
            session.CurrentQuestion = session.Questions.Count - 1;

            return Ok(question);
        }

        [HttpPost("answer")]
        public IActionResult SubmitAnswer([FromQuery] Guid sessionId, [FromBody] string answer)
        {
            if (!Sessions.TryGetValue(sessionId, out var session)) return NotFound();

            var currentQuestion = session.Questions.ElementAtOrDefault(session.CurrentQuestion);
            if (currentQuestion == null) return BadRequest("Ingen giltig fråga hittades.");

            var isCorrect = answer == currentQuestion.CorrectAnswer;
            if (isCorrect)
            {
                session.Stars++;
            }

            if (session.Stars >= 5)
            {
                return Ok(new GameResult
                {
                    GameOver = true,
                    Stars = session.Stars,
                    Level = session.Level,
                    LevelCleared = true
                });
            }

            return Ok(new { correct = isCorrect });
        }

        [HttpPost("progress")]
        public async Task<IActionResult> SubmitProgress([FromBody] GameResult result)
        {
            var playerId = User.Identity?.Name ?? "guest";

            _db.GameProgress.Add(new GameProgressEntity
            {
                PlayerId = playerId,
                GameKey = "WhatsMissing",
                Stars = result.Stars,
                Level = result.Level,
                PlayedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Pong från WhatsMissing");
        }
    }
}
