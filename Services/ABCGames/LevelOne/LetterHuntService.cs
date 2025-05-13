using Lek8LarBackend.Models.LetterHuntModels.LevelOne;

namespace Lek8LarBackend.Services.LetterHunt.LevelOne
{
    public class LetterHuntService
    {
        private static readonly List<(string Letter, string Image)> LetterImagePairs = new()
        {
            ("A", "apa.png"),
            ("B", "bil.png"),
            ("S", "sol.png"),
            ("K", "katt.png"),
            ("O", "ost.png"),
            ("G", "glass.png"),
            ("V", "våffla.png"),
            ("E", "elefant.png"),
          
        };

        public LevelOneLetterHuntGame GenerateQuestion(int level)
        {
            var rand = new Random();
            var pair = LetterImagePairs[rand.Next(LetterImagePairs.Count)];

            var correctLetter = pair.Letter;
            var imageUrl = pair.Image;

            var allLetters = LetterImagePairs.Select(p => p.Letter).Distinct().ToList();
            var options = new List<string> { correctLetter };

            var fullWord = Path.GetFileNameWithoutExtension(imageUrl);
            var partialWord = fullWord.Substring(1);

            while (options.Count < 3)
            {
                var distractor = allLetters[rand.Next(allLetters.Count)];
                if (!options.Contains(distractor))
                    options.Add(distractor);
            }

            options = options.OrderBy(_ => rand.Next()).ToList();

            return new LevelOneLetterHuntGame
            {
                Id = rand.Next(1000, 9999),
                ImageUrl = imageUrl,
                CorrectLetter = correctLetter,
                Options = options,
                PartialWord = partialWord
            };
        }
    }
}
