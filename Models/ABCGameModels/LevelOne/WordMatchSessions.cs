namespace Lek8LarBackend.Models.WordMatchModels.LevelOne
{
    public class WordMatchSession
    {
        public Guid SessionId { get; set; } = Guid.NewGuid();
        public string PlayerId { get; set; } = "";
        public int Level { get; set; }
        public int StarsEarned { get; set; } = 0;
        public int CurrentQuestionNumber { get; set; } = 1;
        public int TotalQuestions { get; set; } = 5;
        public List<LevelOneWordMatchGame> Questions { get; set; } = new();

        public bool LevelCleared => StarsEarned >= 3;
    }
}
