using BlueNotation.Music;

namespace BlueNotation.Data;

public class Statistics
{
    public int TotalNotesPlayed { get; set; } = 0;
    public int TotalNotesAttempted { get; set; } = 0;
    public DateTime TimeCreated { get; set; } = DateTime.Now;
    public List<NoteStatisticsItem> TrebleNotes { get; set; } = new();
    public List<NoteStatisticsItem> BassNotes { get; set; } = new();
    public List<KeyStatisticsItem> Keys { get; set; } = new();

    private readonly Dictionary<int, NoteStatisticsItem> _trebleNoteDictionary = new();
    private readonly Dictionary<int, NoteStatisticsItem> _bassNoteDictionary = new();
    private readonly Dictionary<Key, KeyStatisticsItem> _keyDictionary = new();

    public void UnloadData()
    {
        _trebleNoteDictionary.Clear();
        _bassNoteDictionary.Clear();
        _keyDictionary.Clear();

        foreach (var item in TrebleNotes)
        {
            _trebleNoteDictionary.Add(item.Midi, item);
        }

        foreach (var item in BassNotes)
        {
            _bassNoteDictionary.Add(item.Midi, item);
        }

        foreach (var item in Keys)
        {
            _keyDictionary.Add(item.Key, item);
        }
    }

    public void LoadData()
    {
        TrebleNotes.Clear();
        BassNotes.Clear();
        Keys.Clear();

        foreach (var note in _trebleNoteDictionary.Values)
        {
            TrebleNotes.Add(note);
        }

        foreach (var note in _bassNoteDictionary.Values)
        {
            BassNotes.Add(note);
        }

        foreach (var key in _keyDictionary.Values)
        {
            Keys.Add(key);
        }
    }

    public void AddTrebleNote(NoteStatisticsItem note)
    {
        _trebleNoteDictionary[note.Midi] = note;
    }

    public void AddBassNote(NoteStatisticsItem note)
    {
        _bassNoteDictionary[note.Midi] = note;
    }

    public void AddKey(KeyStatisticsItem key)
    {
        _keyDictionary[key.Key] = key;
    }

    public StatisticsItem GetTrebleNote(int midi)
    {
        if (!_trebleNoteDictionary.ContainsKey(midi))
        {
            _trebleNoteDictionary.Add(midi, new() { Midi = midi });
        }

        return _trebleNoteDictionary[midi];
    }

    public StatisticsItem GetBassNote(int midi)
    {
        if (!_bassNoteDictionary.ContainsKey(midi))
        {
            _bassNoteDictionary.Add(midi, new() { Midi = midi });
        }

        return _bassNoteDictionary[midi];
    }

    public StatisticsItem GetKey(Key key)
    {
        if (!_keyDictionary.ContainsKey(key))
        {
            _keyDictionary.Add(key, new() { Key = key });
        }

        return _keyDictionary[key];
    }
}