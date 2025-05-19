namespace Lek8LarBackend.Models.MemoryGameModels
{
    public class WhatsMissingSession
    {
        public Guid SessionId { get; set; } = Guid.NewGuid();
        public string PlayerId { get; set; } = "guest";
        public int CurrentQuestion { get; set; } = 0;
        public int Stars { get; set; } = 0;
        public int MaxQuestions { get; set; } = 5;

        public int Level { get; set; } = 1;

        public List<WhatsMissingQuestion> Questions { get; set; } = new();
    }

    public class WhatsMissingQuestion
    {
        public List<string> AllImages { get; set; } = new();
        public List<string> RemainingImages { get; set; } = new();
        public List<string> Options { get; set; } = new();
        public string CorrectAnswer { get; set; } = string.Empty;
    }

    public class GameResult
    {
        public bool GameOver { get; set; }
        public int Stars { get; set; }
        public int Level { get; set; } = 1;
        public bool LevelCleared { get; set; }
    }
}