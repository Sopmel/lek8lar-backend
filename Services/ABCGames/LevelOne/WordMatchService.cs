using Lek8LarBackend.Models.WordMatchModels.LevelOne;

namespace Lek8LarBackend.Services.WordMatch.LevelOne
{
    public class WordMatchService
    {
        private static readonly List<(string Main, string Correct, string[] Options)> ImageData = new()
        {
            ("strumpa.png", "fot.png", new[] { "fot.png", "katt.png", "apple.png" }),
            ("hund.png", "ben.png", new[] { "ben.png", "glass.png", "våffla.png" }),
            ("sol.png", "brillor.png", new[] { "brillor.png", "bil.png", "munk.png" }),
            ("apa.png", "banana.png", new[] { "banana.png", "paj.png", "elefant.png" }),
            ("blomma.png", "vattenkanna.png", new[] { "vattenkanna.png", "juicebox.png", "fisk.png" }),
        };

        public LevelOneWordMatchGame GenerateQuestion(int level)
        {
            var rand = new Random();
            var (main, correct, allOptions) = ImageData[rand.Next(ImageData.Count)];

            var options = allOptions.OrderBy(_ => rand.Next()).ToList();

            return new LevelOneWordMatchGame
            {
                Id = rand.Next(1000, 9999),
                ImageUrl = main,
                CorrectImageName = correct,
                Options = options
            };
        }
    }
}
