namespace Lek8LarBackend.Models.MathGameModels.LevelOne
{
    public class LevelOneCountGame
    {
        public int Id { get; set; }
        public string ObjectImageUrl { get; set; } = string.Empty;
        public int ObjectCount { get; set; }
        public List<int> Options { get; set; } = new();
        public int CorrectAnswer { get; set; }
    }

}

namespace Lek8LarBackend.Models.MathGameModels.LevelOne
{
    public class CountGameProgressDto
    {
        public int Level { get; set; }
        public int Stars { get; set; }
        public bool GameOver { get; set; }
        public bool LevelCleared { get; set; }
    }
}
