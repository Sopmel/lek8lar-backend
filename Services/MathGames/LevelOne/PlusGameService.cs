using Lek8LarBackend.Models.MathGameModels.LevelOne;

namespace Lek8LarBackend.Services.MathGames.LevelOne
{
    public class PlusGameService
    {
        private readonly Random _random = new();

        private readonly string[] imageOptions = new[]
   {
            "groda.png",
            "katt.png",
            "hund.png",
            "gris.png",
            "piga.png"
        };

        public LevelOnePlusGame GenerateQuestion(int level)
        {
            var countA = _random.Next(1, 4); // 1–3
            var countB = _random.Next(1, 3); // 1–2
            var correct = countA + countB;

            var options = new HashSet<int> { correct };
            while (options.Count < 3)
            {
                var candidate = _random.Next(1, 6);
                options.Add(candidate);
            }

            var chosenImage = imageOptions[_random.Next(imageOptions.Length)];

            return new LevelOnePlusGame
            {
                Id = _random.Next(1000),
                ObjectImageUrl = chosenImage,
                GroupA = countA,
                GroupB = countB,
                Options = options.OrderBy(_ => _random.Next()).ToList()
            };
        }
    }
}
