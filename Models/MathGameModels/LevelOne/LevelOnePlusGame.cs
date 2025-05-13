using System.Collections.Generic;

namespace Lek8LarBackend.Models.MathGameModels.LevelOne
{
    public class LevelOnePlusGame
    {
        public int Id { get; set; }

        public string ObjectImageUrl { get; set; } = "frog.png";

        public int GroupA { get; set; }

        public int GroupB { get; set; }

        public int ObjectCount => GroupA + GroupB;

        public int CorrectAnswer => ObjectCount;

        public List<int> Options { get; set; } = new();

        public GameResult? GameResult { get; set; }
    }

    public class GameResult
    {
        public bool GameOver { get; set; }
        public int Stars { get; set; }
        public int Level { get; set; }
        public bool LevelCleared { get; set; }
    }
}
