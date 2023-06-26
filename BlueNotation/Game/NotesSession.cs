using BlueNotation.Data;
using BlueNotation.Music;

namespace BlueNotation.Game;

public class NotesSession : ISession
{
    private readonly NotesSessionPreset _preset;
    private readonly Random _random = new();

    private readonly Dictionary<int, StatisticsItemHistory> _trebleNoteStats = new();
    private readonly Dictionary<int, StatisticsItemHistory> _bassNoteStats = new();
    private readonly Queue<int> _notes = new();
    private int _currentAttempts = 0;

    public bool UseTrebleClef { get; private set; } = true;
    public Key Key { get; } = new(Letter.C, Accidental.Natural);
    public int TotalNotesPlayed { get; private set; } = 0;
    public int TotalAttempts { get; private set; } = 0;
    public int TotalPerfect { get; private set; } = 0;

    public NotesSession(NotesSessionPreset preset)
    {
        if (preset.MinNotes == 0)
        {
            throw new ArgumentException("Minimum notes must be at least one.", nameof(preset));
        }
        if (preset.MaxNotes == 0)
        {
            throw new ArgumentException("Maximum notes must be at least one.", nameof(preset));
        }
        if (preset.MaxNotes < preset.MinNotes)
        {
            throw new ArgumentException("Minimum notes must be smaller or equal to maximum notes.", nameof(preset));
        }
        if (preset.TrebleNoteRange.Count == 0)
        {
            throw new ArgumentException("Must provide at least one starting treble note.", nameof(preset));
        }
        if (preset.BassNoteRange.Count == 0)
        {
            throw new ArgumentException("Must provide at least one starting bass note.", nameof(preset));
        }

        _preset = preset;

        NextNote();
    }

    private void NextNote()
    {
        _currentAttempts = 0;

        if (_notes.Count <= 1)
        {
            var oldNote = _notes.Count == 1 ? _notes.Peek() : int.MinValue;
            var oldClef = UseTrebleClef;

            var clef = true;
            if (_preset.ClefMode == ClefMode.Bass)
            {
                clef = false;
            }
            if (_preset.ClefMode == ClefMode.Both)
            {
                clef = _random.Next(2) == 1;
            }

            var range = clef ? _preset.TrebleNoteRange : _preset.BassNoteRange;

            var count = _random.Next(_preset.MinNotes, _preset.MaxNotes + 1);

            bool requireUnique = count == 1 && !_preset.AllowRepeats && range.Count > 1 && clef == oldClef;

            _notes.Clear();

            if (requireUnique)
            {
                int chosenNote;
                do
                {
                    chosenNote = range[_random.Next(range.Count)];
                }
                while (requireUnique && chosenNote == oldNote);

                _notes.Enqueue(chosenNote);
                UseTrebleClef = clef;
                return;
            }

            var copy = range.ToList();
            var unordered = new List<int>();

            while (count > 0 && copy.Count > 0)
            {
                count--;

                var chosenNote = copy[_random.Next(copy.Count)];
                copy.Remove(chosenNote);
                unordered.Add(chosenNote);
            }

            foreach (var note in unordered.Order())
            {
                _notes.Enqueue(note);
            }

            UseTrebleClef = clef;
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

            var noteStats = UseTrebleClef ? _trebleNoteStats : _bassNoteStats;

            if (!noteStats.ContainsKey(midi))
            {
                noteStats[midi] = new();
            }

            var old = noteStats[midi];

            noteStats[midi] = new(old.Attempts + _currentAttempts, old.TimesPlayed + 1, old.Latency + latency);

            if (_currentAttempts == 1)
            {
                TotalPerfect++;
            }

            NextNote();
            return true;
        }

        return false;
    }

    public void ApplyStatistics(Statistics statistics)
    {
        statistics.TotalNotesPlayed += TotalNotesPlayed;
        statistics.TotalNotesAttempted += TotalAttempts;

        foreach (var pair in _trebleNoteStats)
        {
            var note = statistics.GetTrebleNote(pair.Key);

            note.AddData(pair.Value);
        }

        foreach (var pair in _bassNoteStats)
        {
            var note = statistics.GetBassNote(pair.Key);

            note.AddData(pair.Value);
        }
    }

    public IEnumerable<KeyValuePair<Note, StatisticsItemHistory>> GetTrebleNoteStatistics()
    {
        return _trebleNoteStats.Select(kvp => new KeyValuePair<Note, StatisticsItemHistory>(NoteHelper.GetNote(kvp.Key, Key), kvp.Value));
    }

    public IEnumerable<KeyValuePair<Note, StatisticsItemHistory>> GetBassNoteStatistics()
    {
        return _bassNoteStats.Select(kvp => new KeyValuePair<Note, StatisticsItemHistory>(NoteHelper.GetNote(kvp.Key, Key), kvp.Value));
    }
}
