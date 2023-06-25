namespace BlueNotation.Game;

public class PresetsData
{
    public List<NotesSessionPreset> NotePresets { get; set; } = new() { new() };
    public List<KeysSessionPreset> KeyPresets { get; set; } = new();

    public NotesSessionPreset? GetNotesPreset(string name)
    {
        foreach (var preset in NotePresets)
        {
            if (preset.Name == name)
            {
                return preset;
            }
        }

        return null;
    }

    public void DeleteNotesPreset(string name)
    {
        NotePresets = NotePresets.Where(p => p.Name != name).ToList();
    }

    public void AddNotesPreset(NotesSessionPreset preset)
    {
        DeleteKeysPreset(preset.Name);
        NotePresets.Add(preset);
    }

    public KeysSessionPreset? GetKeysPreset(string name)
    {
        foreach (var preset in KeyPresets)
        {
            if (preset.Name == name)
            {
                return preset;
            }
        }

        return null;
    }

    public void DeleteKeysPreset(string name)
    {
        KeyPresets = KeyPresets.Where(p => p.Name != name).ToList();
    }

    public void AddKeysPreset(KeysSessionPreset preset)
    {
        DeleteKeysPreset(preset.Name);
        KeyPresets.Add(preset);
    }
}
