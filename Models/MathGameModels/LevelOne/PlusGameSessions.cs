using System;
using System.Collections.Generic;
using Lek8LarBackend.Models.MathGameModels.LevelOne;

namespace Lek8LarBackend.Models
{
    public class PlusGameSession
    {
        public Guid SessionId { get; set; } = Guid.NewGuid();
        public string PlayerId { get; set; } = "";
        public int Level { get; set; } = 1;
        public int CurrentQuestionNumber { get; set; } = 1;
        public int StarsEarned { get; set; } = 0;
        public int TotalQuestions { get; set; } = 5;
        public List<LevelOnePlusGame> Questions { get; set; } = new();
    }
}
