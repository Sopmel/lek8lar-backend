namespace Lek8LarBackend.Models.MathGameModels.LevelOne
{
    public class LevelOneShapeGame
    {
        public int Id { get; set; }
        public Guid SessionId { get; set; }
        public string ShapeImageUrl { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
        public string CorrectAnswer { get; set; } = string.Empty;
        public bool GameOver { get; set; }
    }
}
