namespace Lek8LarBackend.Models.LetterHuntModels.LevelOne
{
    public class LevelOneLetterHuntGame
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = "";
        public string CorrectLetter { get; set; } = "";
        public List<string> Options { get; set; } = new();
        public string PartialWord { get; set; }
    }
}
