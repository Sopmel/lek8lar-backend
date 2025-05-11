using Lek8LarBackend.Models.MathGameModels.LevelOne;

namespace Lek8LarBackend.Services.MathGames.LevelOne
{
    public class CountGameService
    {
        private readonly Random _random = new();

        public LevelOneCountGame GenerateQuestion(int level)
        {
            int maxCount = level switch
            {
                1 => 5,
                2 => 8,
                3 => 10,
                _ => 5
            };

            int count = _random.Next(1, maxCount + 1);
            var correct = count;

            var options = new List<int> { correct };
            var imageOptions = new[] { "apple.png", "banana.png", "ball.png" };
            string chosenImage = imageOptions[_random.Next(imageOptions.Length)];

            while (options.Count < 3)
            {
                int opt = _random.Next(1, maxCount + 1);
                if (!options.Contains(opt)) options.Add(opt);
            }

            options = options.OrderBy(_ => _random.Next()).ToList();

            return new LevelOneCountGame
            {
                Id = _random.Next(1000),
                ObjectImageUrl = chosenImage,
                ObjectCount = count,
                Options = options,
                CorrectAnswer = correct
            };
        }

    }

}
