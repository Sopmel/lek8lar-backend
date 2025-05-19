using Microsoft.AspNetCore.Mvc;
using Lek8LarBackend.Data;
using Lek8LarBackend.Data.Entities;
using Lek8LarBackend.Models.MemoryGameModels.LevelOne;
using System.Collections.Concurrent;

namespace Lek8LarBackend.Controllers.MemoryGameControllers.LevelOne
{
    [ApiController]
    [Route("memorygame")]
    public class MemoryGameController : ControllerBase
    {
        private static readonly ConcurrentDictionary<Guid, MemoryGameSession> Sessions = new();
        private readonly ApplicationDbContext _db;

        public MemoryGameController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost("start")]
        public IActionResult StartGame()
        {
            var session = new MemoryGameSession
            {
                PlayerId = User.Identity?.Name ?? "guest",
                Level = 1
            };

            Sessions[session.SessionId] = session;

            return Ok(new { sessionId = session.SessionId });
        }

        [HttpGet("cards")]
        public IActionResult GetCards([FromQuery] Guid sessionId)
        {
            if (!Sessions.TryGetValue(sessionId, out var session))
                return NotFound();

            var images = new[] { "apple.png", "banana.png", "sol.png", "blomma.png", "ball.png" };
            var deck = images
                .Concat(images)
                .Select((img, i) => new MemoryCard
                {
                    Id = i,
                    Image = img
                })
                .OrderBy(_ => Guid.NewGuid())
                .ToList();

            return Ok(deck);
        }

    }
}
