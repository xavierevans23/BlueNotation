using BlueNotation.Game;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlueNotation.Popups;

public partial class SaveCurrent
{
    private bool _success = false;
    private string _name = "";

    [CascadingParameter] MudDialogInstance? MudDialog { get; set; }

    private async Task Submit()
    {
        if (ConductorService.Preset is NotesSessionPreset notes)
        {
            notes.Name = _name;
            DataService.PresetsData.AddNotesPreset(notes);
        }
        if (ConductorService.Preset is KeysSessionPreset keys)
        {
            keys.Name = _name;
            DataService.PresetsData.AddKeysPreset(keys);
        }

        await DataService.SavePresets();

        MudDialog?.Close(DialogResult.Ok(true));
    }
}
