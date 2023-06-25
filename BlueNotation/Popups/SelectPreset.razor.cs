using BlueNotation.Game;

namespace BlueNotation.Popups;

public partial class SelectPreset
{
    private List<SessionPreset> _presets = new();

    protected override void OnParametersSet()
    {
        _presets = new();

        foreach (var notePreset in DataService.PresetsData.NotePresets)
        {
            _presets.Add(notePreset);
        }

        foreach (var keyPreset in DataService.PresetsData.KeyPresets)
        {
            _presets.Add(keyPreset);
        }
    }
}
