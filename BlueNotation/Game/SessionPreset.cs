namespace BlueNotation.Game;

public abstract class SessionPreset
{
    public EndMode EndMode { get; set; } = EndMode.QuestionCount;
    public int TimerSeconds { get; set; } = 60;
    public int QuestionCount { get; set; } = 10;
    public ClefMode ClefMode { get; set; } = ClefMode.Treble;
    public bool AllowRepeats { get; set; } = false;
    public List<int> TrebleNoteRange { get; set; } = new() { 60, 62, 64, 65, 67, 69, 71, 72 };
    public List<int> BassNoteRange { get; set; } = new() { 48, 50, 52, 53, 55, 57, 59, 71 };
    public string Name { get; set; } = "Test";
}

public enum EndMode
{
    Timer,
    QuestionCount,
    Infinite
}

public enum ClefMode
{
    Treble,
    Bass,
    Both
}