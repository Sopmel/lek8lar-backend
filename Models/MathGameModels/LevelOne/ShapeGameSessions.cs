namespace Lek8LarBackend.Models.MathGameModels.LevelOne
{
    public class ShapeGameSession
    {
        public Guid SessionId { get; set; } = Guid.NewGuid();
        public string PlayerId { get; set; } = "guest"; 
        public int TotalQuestions { get; set; } = 5;
        public int CurrentQuestionNumber { get; set; } = 1;
        public int StarsEarned { get; set; } = 0;
        public bool LevelCleared => StarsEarned == TotalQuestions;
        public List<LevelOneShapeGame> Questions { get; set; } = new();
        public string CurrentCorrectAnswer { get; set; } = string.Empty; 
    }
}
