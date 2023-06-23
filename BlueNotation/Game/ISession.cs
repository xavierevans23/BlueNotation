using BlueNotation.Music;
using BlueNotation.Data;

namespace BlueNotation.Game;

public interface ISession
{
    public Key Key { get; }
    public bool UseTrebleClef { get; }
    public int TotalNotesPlayed { get; }
    public int TotalAttempts { get; }
    public IEnumerable<Note> GetNotes();
    public bool NotePlayed(int midi, int latency);
    public void ApplyStatistics(Statistics statistics);
}
