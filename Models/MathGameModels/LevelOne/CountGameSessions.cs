namespace Lek8LarBackend.Models.MathGameModels.LevelOne
{
    public class CountGameSession
    {
        public Guid SessionId { get; set; } = Guid.NewGuid();
        public string PlayerId { get; set; } = "guest"; // kopplas till inloggad användare sen
        public int TotalQuestions { get; set; } = 5;
        public int CurrentQuestionNumber { get; set; } = 1;
        public int StarsEarned { get; set; } = 0;
        public bool LevelCleared => StarsEarned == TotalQuestions;
        public List<LevelOneCountGame> Questions { get; set; } = new();
    }

}
