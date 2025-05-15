using Microsoft.AspNetCore.Mvc;
using Lek8LarBackend.Models.LetterBubbleModels.LevelOne;
using Lek8LarBackend.Services.LetterBubbleGames.LevelOne;
using System.Collections.Concurrent;

namespace Lek8LarBackend.Controllers.LetterBubbleControllers.LevelOne;

[ApiController]
[Route("api/letterbubble")]
public class LetterBubbleController : ControllerBase
{
    private static readonly ConcurrentDictionary<Guid, LetterBubbleSession> Sessions = new();
    private readonly LetterBubbleService _service = new();

    [HttpPost("start")]
    public IActionResult StartGame([FromQuery] int level = 1)
    {
        var playerId = User.Identity?.Name ?? "guest";

        var session = new LetterBubbleSession
        {
            PlayerId = playerId,
            Level = level,
            TotalQuestions = 5
        };

        for (int i = 0; i < session.TotalQuestions; i++)
        {
            var question = _service.GenerateQuestion(i + 1);
            session.Questions.Add(question);
        }

        Sessions[session.SessionId] = session;

        return Ok(new
        {
            sessionId = session.SessionId,
            totalQuestions = session.TotalQuestions
        });
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

            return Ok(new LevelOneLetterBubbleGame
            {
                Id = -1,
                TargetLetter = "",
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
    public IActionResult SubmitAnswer([FromBody] LetterBubbleAnswerRequest request)
    {
        if (!Sessions.TryGetValue(request.SessionId, out var session))
            return NotFound();

        var currentIndex = session.CurrentQuestionNumber - 1;
        var question = session.Questions[currentIndex];

        bool isCorrect = request.Answer == question.TargetLetter;
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

    public class LetterBubbleAnswerRequest
    {
        public Guid SessionId { get; set; }
        public string Answer { get; set; } = string.Empty;
    }
}
