namespace Lek8LarBackend.Models.LetterBubbleModels.LevelOne;

public class LetterBubbleSession
{
    public Guid SessionId { get; set; } = Guid.NewGuid();
    public string PlayerId { get; set; } = string.Empty;
    public int Level { get; set; }
    public int TotalQuestions { get; set; } = 5;
    public int CurrentQuestionNumber { get; set; } = 1;
    public int StarsEarned { get; set; } = 0;
    public List<LevelOneLetterBubbleGame> Questions { get; set; } = new();
}
