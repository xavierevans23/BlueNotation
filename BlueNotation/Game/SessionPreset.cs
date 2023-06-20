namespace BlueNotation.Game;

public abstract class SessionPreset
{
    public EndMode EndMode { get; set; } = EndMode.QuestionCount;
    public int TimerSeconds { get; set; } = 60;
    public int QuestionCount { get; set; } = 10;
    public CleffMode CleffMode { get; set; } = CleffMode.Treble;
    public bool AllowRepeats { get; set; } = false;
}

public enum EndMode
{
    Timer,
    QuestionCount,
    Infinite
}

public enum CleffMode
{
    Treble,
    Bass,
    Both
}