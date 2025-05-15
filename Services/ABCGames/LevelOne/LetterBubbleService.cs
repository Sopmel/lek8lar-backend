using Lek8LarBackend.Models.LetterBubbleModels.LevelOne;

namespace Lek8LarBackend.Services.LetterBubbleGames.LevelOne;

public class LetterBubbleService
{
    private static readonly Random Random = new();

    private static readonly List<string> Letters = new()
    {
        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J"
    };

    public LevelOneLetterBubbleGame GenerateQuestion(int id)
    {
        var correct = Letters[Random.Next(Letters.Count)];

        var options = new HashSet<string> { correct };
        while (options.Count < 3)
        {
            options.Add(Letters[Random.Next(Letters.Count)]);
        }

        return new LevelOneLetterBubbleGame
        {
            Id = id,
            TargetLetter = correct,
            Options = options.OrderBy(_ => Random.Next()).ToList()
        };
    }
}
