namespace Lek8LarBackend.Data.Entities
{
    public class MemoryGameProgressEntity
    {
        public int Id { get; set; }
        public string PlayerId { get; set; } = string.Empty;
        public int Level { get; set; }
        public int Stars { get; set; }
        public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
    }
}
