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
