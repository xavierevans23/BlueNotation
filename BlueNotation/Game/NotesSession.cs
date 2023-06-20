using BlueNotation.Data;
using BlueNotation.Music;

namespace BlueNotation.Game;

public class NotesSession : ISession
{
    private readonly NotesSessionPreset _preset;
    private readonly Random _random = new();

    private readonly Dictionary<int, StatisticsItemHistory> _noteStats = new();
    private readonly Queue<int> _notes = new();
    private int _currentAttempts = 0;

    public bool UseTrebleCleff { get; private set; } = true;
    public Key Key { get; } = new(Letter.C, Accidental.Natural);
    public int TotalNotesPlayed { get; private set; } = 0;
    public int TotalAttempts { get; private set; } = 0;

    public NotesSession(NotesSessionPreset preset)
    {
        _preset = preset;

        NextNote();
    }

    private void NextNote()
    {
        _currentAttempts = 0;
        
        if (_notes.Count <= 1)
        {
            var oldNote = _notes.Count == 1 ? _notes.Peek() : int.MinValue;
            var oldCleff = UseTrebleCleff;

            var cleff = true;
            if (_preset.CleffMode == CleffMode.Bass)
            {
                cleff = false;
            }
            if (_preset.CleffMode == CleffMode.Both)
            {
                cleff = _random.Next(2) == 1;
            }

            var range = cleff ? _preset.TrebleNoteRange : _preset.BassNoteRange;

            var count = _random.Next(_preset.MinNotes, _preset.MaxNotes + 1);

            bool requireUnique = count == 1 && !_preset.AllowRepeats && range.Count > 1 && cleff == oldCleff;

            _notes.Clear();

            if (requireUnique)
            {
                var chosenNote = 0;

                do
                {
                    chosenNote = range[_random.Next(range.Count)];
                }
                while (requireUnique && chosenNote == oldNote);

                _notes.Enqueue(chosenNote);
                UseTrebleCleff = cleff;
                return;
            }

            var copy = range.ToList();

            while (count > 0 && copy.Count > 0)
            {
                count--;

                var chosenNote = copy[_random.Next(copy.Count)];
                copy.Remove(chosenNote);
                _notes.Enqueue(chosenNote);
            }

            UseTrebleCleff = cleff;
            return;
        }

        _notes.Dequeue();
    }

    public IEnumerable<Note> GetNotes() => _notes.Select(n => NoteHelper.GetNote(n, Key));

    public bool NotePlayed(int midi, int latency) 
    {
        _currentAttempts++;

        if (midi == _notes.Peek())
        {
            TotalNotesPlayed++;
            TotalAttempts += _currentAttempts;

            if (!_noteStats.ContainsKey(midi))
            {
                _noteStats[midi] = new();
            }

            var old = _noteStats[midi];

            _noteStats[midi] = new(old.Attempts + _currentAttempts, old.TimesPlayed + 1, old.Latency + latency);

            NextNote();
            return true;
        }

        return false;
    }

    public void ApplyStatistics(Statistics statistics) 
    {
        statistics.TotalNotesPlayed+= TotalNotesPlayed;
        statistics.TotalNotesAttempted += TotalAttempts;

        foreach (var pair in _noteStats)
        {
            var note = statistics.GetNote(pair.Key);

            note.AddData(pair.Value);
        }
    }
}
