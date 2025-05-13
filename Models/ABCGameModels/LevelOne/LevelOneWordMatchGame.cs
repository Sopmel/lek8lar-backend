namespace Lek8LarBackend.Models.WordMatchModels.LevelOne
{
    public class LevelOneWordMatchGame
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = "";
        public string CorrectImageName { get; set; } = "";
        public List<string> Options { get; set; } = new();
    }
}
