public class GameProgressDto
{
    public string GameKey { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Stars { get; set; }
    public bool GameOver { get; set; }
    public bool LevelCleared { get; set; }
}
