
public class ColorMixQuestion
{
    public int Id { get; set; }
    public string Color1 { get; set; } = string.Empty;
    public string Color2 { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();
    public string CorrectAnswer { get; set; } = string.Empty;
}
