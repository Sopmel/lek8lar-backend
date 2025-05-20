using System.Collections.Concurrent;
using Lek8LarBackend.Models.MemoryGameModels;


namespace Lek8LarBackend.Services.MemoryGames;
 public class ColorMixGameService
{
    private static readonly ConcurrentDictionary<string, ColorMixGameSession> Sessions = new();

    private static readonly List<ColorMixQuestion> AllQuestions = new()
        {
            new ColorMixQuestion { Id = 1, Color1 = "red", Color2 = "yellow", Options = new() { "orange", "green", "purple" }, CorrectAnswer = "orange" },
            new ColorMixQuestion { Id = 2, Color1 = "blue", Color2 = "red", Options = new() { "purple", "green", "orange" }, CorrectAnswer = "purple" },
            new ColorMixQuestion { Id = 3, Color1 = "yellow", Color2 = "blue", Options = new() { "green", "pink", "orange" }, CorrectAnswer = "green" },
            new ColorMixQuestion { Id = 4, Color1 = "white", Color2 = "black", Options = new() { "gray", "blue", "pink" }, CorrectAnswer = "gray" },
            new ColorMixQuestion { Id = 5, Color1 = "red", Color2 = "white", Options = new() { "pink", "purple", "gray" }, CorrectAnswer = "pink" },
        };

    public ColorMixGameSession StartNewSession(int level)
    {
        var session = new ColorMixGameSession
        {
            Level = level,
            Questions = AllQuestions.OrderBy(_ => Guid.NewGuid()).Take(5).ToList()
        };

        Sessions[session.SessionId] = session;
        return session;
    }

    public ColorMixQuestion? GetNextQuestion(string sessionId)
    {
        if (!Sessions.TryGetValue(sessionId, out var session)) return null;
        if (session.Round >= session.Questions.Count) return null;

        return session.Questions[session.Round];
    }

    public (bool correct, bool gameOver, int stars) SubmitAnswer(string sessionId, string answer)
    {
        if (!Sessions.TryGetValue(sessionId, out var session)) return (false, true, session.Stars);
        if (session.GameOver || session.Round >= session.Questions.Count) return (false, true, session.Stars);

        var current = session.Questions[session.Round];
        var correct = current.CorrectAnswer.Equals(answer, StringComparison.OrdinalIgnoreCase);

        if (correct)
            session.Stars++;

        session.Round++;

        if (session.Round >= session.Questions.Count)
            session.GameOver = true;

        return (correct, session.GameOver, session.Stars);
    }

    public void EndGame(string sessionId, int stars)
    {
        if (Sessions.TryGetValue(sessionId, out var session))
        {
            session.Stars = stars;
            session.GameOver = true;
        }
    }
}