using BlueNotation.Data;
using BlueNotation.Music;

namespace BlueNotation.Game;

public class KeysSession
{
    private readonly Random _random = new();
    private readonly KeysSessionPreset _preset;

    private int[] _scale = Array.Empty<int>();
    private int _position = 0;
    private int _elapsedLatency = 0;
    private int _elapsedAttempts = 1;
    private readonly Dictionary<Key, StatisticsItemHistory> _scaleHistory = new();

    public Key Key { get; private set; } = new(Letter.C, Accidental.Natural);
    public bool UseTrebleClef { get; private set; } = true;
    public int TotalNotesPlayed { get; private set; } = 0;
    public int TotalAttempts { get; private set; } = 0;
    public int TotalScalesPlayed { get; private set; } = 0;

    public KeysSession(KeysSessionPreset preset)
    {
        if (preset.Keys.Count == 0)
        {
            throw new ArgumentException("Must provide at least one key.", nameof(preset));
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

        GetNextNote(true);        
    }

    public IEnumerable<Note> GetNotes()
    {
        yield return NoteHelper.GetNote(_scale[_position], Key);
    }

    private void GetNextNote(bool first = false)
    {
        if (_position > 0 && _scale.Length > 0)
        {
            _position--;
            return;
        }

        var oldKey = Key;

        do
        {
            Key = _preset.Keys[_random.Next(_preset.Keys.Count)];
        }
        while (oldKey == Key && !first && _preset.Keys.Count != 1);

        UseTrebleClef = true;

        if (_preset.ClefMode == ClefMode.Bass)
        {
            UseTrebleClef = false;
        }

        if (_preset.ClefMode == ClefMode.Both)
        {
            UseTrebleClef = _random.Next(2) == 1;
        }

        var range = UseTrebleClef ? _preset.TrebleNoteRange : _preset.BassNoteRange;

        var startPoint = range[_random.Next(range.Count)];

        var notes = NoteHelper.GetScale(startPoint, Key).ToList();

        switch (_preset.Direction)
        {
            case Direction.Up:
                notes.Reverse();
                break;

            case Direction.Both:
                if (_random.Next(2) == 1)
                {
                    notes.Reverse();
                }
                break;

            case Direction.Shuffle:
                var duplicate = notes.ToList();
                notes.Clear();

                while (duplicate.Any())
                {
                    var note = duplicate[_random.Next(duplicate.Count)];
                    notes.Add(note);
                    duplicate.Remove(note);
                }

                break;
        }

        _scale = notes.ToArray();
        _position = 6;
        _elapsedLatency = 0;
        _elapsedAttempts = 1;
    }

    public bool NotePlayed(int midi, int latency)
    {
        TotalAttempts++;
        _elapsedLatency += latency;

        if (midi == _scale[_position])
        {
            TotalNotesPlayed++;

            if (_position == 0)
            {
                TotalScalesPlayed++;

                if (!_scaleHistory.ContainsKey(Key))
                {
                    _scaleHistory.Add(Key, new());
                }

                var item = _scaleHistory[Key];
                _scaleHistory[Key] = new(item.Attempts + _elapsedAttempts, item.TimesPlayed + 1,item.Latency + _elapsedLatency);
            }

            GetNextNote();
            return true;
        }

        _elapsedAttempts++;
        if (_preset.ForceRetry)
        {
            _position = 6;
            _elapsedLatency = 1;
            _elapsedAttempts++;
        }

        return false;
    }

    public void ApplyStatistics(Statistics statistics)
    {
        statistics.TotalNotesPlayed += TotalNotesPlayed;
        statistics.TotalNotesAttempted += TotalAttempts;

        foreach (var pair in _scaleHistory)
        {
            statistics.GetKey(pair.Key).AddData(pair.Value);
        }
    }

    public IEnumerable<KeyValuePair<Key, StatisticsItemHistory>> GetKeyStatistics()
    {
        return _scaleHistory;
    }
}
