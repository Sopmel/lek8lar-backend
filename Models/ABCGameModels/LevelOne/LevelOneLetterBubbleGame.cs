namespace Lek8LarBackend.Models.LetterBubbleModels.LevelOne;

public class LevelOneLetterBubbleGame
{
    public int Id { get; set; }
    public string TargetLetter { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();
    public GameResult? GameResult { get; set; }
}

public class GameResult
{
    public bool GameOver { get; set; }
    public int Stars { get; set; }
    public int Level { get; set; }
    public bool LevelCleared { get; set; }
    public bool Correct { get; set; }
}
