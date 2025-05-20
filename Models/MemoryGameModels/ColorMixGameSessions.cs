namespace Lek8LarBackend.Models.MemoryGameModels

{
    public class ColorMixGameSession
    {
        public string SessionId { get; set; } = Guid.NewGuid().ToString();
        public int Round { get; set; } = 0;
        public int Stars { get; set; } = 0;
        public int Level { get; set; } = 1;
        public bool GameOver { get; set; } = false;

        public List<ColorMixQuestion> Questions { get; set; } = new();
    }
}