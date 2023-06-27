using BlueNotation.Game;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlueNotation.Popups;

public partial class SelectPreset
{
    private List<SessionPreset> _presets = new();

    [CascadingParameter] MudDialogInstance? MudDialog { get; set; }

    private async Task Select(SessionPreset preset)
    {
        ConductorService.Preset = preset;

        if (ConductorService.MusicArea is not null)
        {
            await ConductorService.MusicArea.NewPreset();
        }

        MudDialog?.Close(DialogResult.Ok(true));
    }

    private async Task Delete(SessionPreset preset) 
    {
        DataService.PresetsData.DeletePreset(preset);
        await DataService.SavePresets();

        _presets = new();

        foreach (var notePreset in DataService.PresetsData.NotePresets)
        {
            _presets.Add(notePreset);
        }

        foreach (var keyPreset in DataService.PresetsData.KeyPresets)
        {
            _presets.Add(keyPreset);
        }
        await InvokeAsync(StateHasChanged);
    }

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
