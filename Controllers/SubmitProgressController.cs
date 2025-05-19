using Lek8LarBackend.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Lek8LarBackend.Data;
using Lek8LarBackend.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/progress")]
public class SubmitProgressController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public SubmitProgressController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitProgress([FromBody] GameProgressDto progress)
    {
        var playerId = User?.Identity?.Name ?? "guest";

        var entity = new GameProgressEntity
        {
            PlayerId = playerId,
            GameKey = progress.GameKey,
            Level = progress.Level,
            Stars = progress.Stars,
            PlayedAt = DateTime.UtcNow
        };

        _db.GameProgress.Add(entity);
        await _db.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetProgress()
    {
        var playerId = User.Identity?.Name ?? "guest";

        var progress = await _db.GameProgress
            .Where(p => p.PlayerId == playerId)
            .Select(p => new
            {
                p.GameKey,
                p.Level,
                p.Stars,
                p.PlayedAt
            })
            .ToListAsync();

        return Ok(new
        {
            playerId,
            progress
        });
    }
}
