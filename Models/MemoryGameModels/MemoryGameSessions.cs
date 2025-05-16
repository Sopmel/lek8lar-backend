namespace Lek8LarBackend.Models.MemoryGameModels.LevelOne
{
    public class MemoryCard
    {
        public int Id { get; set; }
        public string Image { get; set; } = string.Empty;
    }

    public class MemoryGameSession
    {
        public Guid SessionId { get; set; } = Guid.NewGuid();
        public string PlayerId { get; set; } = string.Empty;
        public int Level { get; set; } = 1;
    }

    public class MemoryGameProgressDto
    {
        public int Level { get; set; }
        public int Stars { get; set; }
        public bool GameOver { get; set; }
        public bool LevelCleared { get; set; }
    }
}
