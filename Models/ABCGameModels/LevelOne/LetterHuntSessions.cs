using System;

namespace Lek8LarBackend.Models.LetterHuntModels.LevelOne
{
    public class LetterHuntSession
    {
        public Guid SessionId { get; } = Guid.NewGuid();
        public string PlayerId { get; set; } = "";
        public int Level { get; set; }
        public int CurrentQuestionNumber { get; set; } = 1;
        public int TotalQuestions { get; set; } = 5;
        public int StarsEarned { get; set; } = 0;
        public bool LevelCleared => StarsEarned >= 5;
        public List<LevelOneLetterHuntGame> Questions { get; set; } = new();
    }
}
