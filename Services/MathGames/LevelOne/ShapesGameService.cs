using Lek8LarBackend.Models.MathGameModels.LevelOne;
using System;
using System.Collections.Generic;

namespace Lek8LarBackend.Services.MathGames.LevelOne
{
    public class ShapeGameService
    {
        private readonly Random _random = new();

        private readonly string[] shapeNames = { "Cirkel", "Kvadrat", "Triangel", "Rektangel", "Stjärna", "Hjärta" };
        private readonly string[] shapeImages = { "circel.png", "kvadrat.png", "triangel.png", "rektangel.png", "stjarna.png", "hjarta.png" };

        // 🧠 Spara sessions internt (eller ersätt med Dependency Injected sessionhantering om det finns)
        private static readonly Dictionary<Guid, ShapeGameSession> sessions = new();

        public LevelOneShapeGame GenerateQuestion(Guid sessionId, int level)
        {
            int maxShapes = Math.Min(level + 2, shapeNames.Length); 

            int index = _random.Next(maxShapes);
            string correctShape = shapeNames[index];
            string image = shapeImages[index];

            var options = new List<string> { correctShape };
            while (options.Count < 3)
            {
                var option = shapeNames[_random.Next(maxShapes)];
                if (!options.Contains(option)) options.Add(option);
            }

            options = options.OrderBy(_ => _random.Next()).ToList();

            var shapeEmojis = new Dictionary<string, string>()
{
    { "Cirkel", "⚪" },
    { "Kvadrat", "🟥" },
    { "Triangel", "🔺" },
    { "Rektangel", "▭" },
    { "Stjärna", "⭐" },
    { "Hjärta", "❤️" }
};


            var visualOptions = options
                .Select(shape => $"{shapeEmojis[shape]} {shape}")
                .ToList();

            if (!sessions.ContainsKey(sessionId))
            {
                sessions[sessionId] = new ShapeGameSession();
            }

            sessions[sessionId].CurrentCorrectAnswer = correctShape;

            return new LevelOneShapeGame
            {
                Id = _random.Next(1000),
                ShapeImageUrl = image,
                CorrectAnswer = $"{shapeEmojis[correctShape]} {correctShape}",
                Options = visualOptions,
                GameOver = false,
                SessionId = sessionId
            };
        }


        public bool CheckAnswer(string answerWithEmoji, Guid sessionId)
        {
            if (!sessions.TryGetValue(sessionId, out var session))
                return false;

            // Ta bort emoji och extra mellanslag
            var parts = answerWithEmoji.Split(' ', 2);
            var cleanedAnswer = parts.Length > 1 ? parts[1] : answerWithEmoji;

            return cleanedAnswer == session.CurrentCorrectAnswer;
        }

        public string GetCorrectAnswer(int questionId, Guid sessionId)
        {
            if (sessions.TryGetValue(sessionId, out var session))
            {
                return session.CurrentCorrectAnswer;
            }

            return string.Empty;
        }
    }
}
