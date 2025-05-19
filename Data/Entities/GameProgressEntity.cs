namespace Lek8LarBackend.Data.Entities
{
    public class GameProgressEntity
    {
        public int Id { get; set; }
        public string PlayerId { get; set; } = string.Empty;
        public string GameKey { get; set; } = string.Empty; // t.ex. "MemoryGame", "CountGame"
        public int Level { get; set; }
        public int Stars { get; set; }
        public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
    }
}
