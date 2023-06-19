using BlueNotation.Music;

namespace BlueNotation.Data;

public class Statistics
{
    public int TotalNotesPlayed { get; set; } = 0;
    public DateTime TimeCreated { get; set; } = DateTime.Now;
    public List<NoteStatisticsItem> Notes { get; set; } = new();
    public List<KeyStatisticsItem> Keys { get; set; } = new();

    private readonly Dictionary<int, NoteStatisticsItem> _noteDictionary = new();
    private readonly Dictionary<Key, KeyStatisticsItem> _keyDictionary = new();

    public void UnloadData()
    {
        _noteDictionary.Clear();
        _keyDictionary.Clear();

        foreach (var item in Notes)
        {
            _noteDictionary.Add(item.Midi, item);
        }

        foreach (var item in Keys)
        {
            _keyDictionary.Add(item.Key, item);
        }
    }

    public void LoadData()
    {
        Notes.Clear();
        Keys.Clear();

        foreach (var note in _noteDictionary.Values)
        {
            Notes.Add(note);
        }

        foreach (var key in _keyDictionary.Values)
        {
            Keys.Add(key);
        }
    }

    public void AddNote(NoteStatisticsItem note)
    {
        _noteDictionary[note.Midi] = note;
    }

    public void AddKey(KeyStatisticsItem key)
    {
        _keyDictionary[key.Key] = key;
    }

    public StatisticsItem GetNote(int midi)
    {
        if (!_noteDictionary.ContainsKey(midi))
        {
            _noteDictionary.Add(midi, new() { Midi = midi });
        }

        return _noteDictionary[midi];
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