using MudBlazor;
using BlueNotation.Services;

namespace BlueNotation.Shared;

public partial class MusicArea
{
    private const string FlashCss = "flash";
    private string _noteText = "Connecting";
    private string _noteTextCss = "";
    private Severity _noteTextSeverity = Severity.Info;
    private bool _noteTextNoIcon = false;

    private async Task ChangeNoteText(string text, Severity severity, bool icon)
    {
        Console.WriteLine(text);

        _noteTextCss = "";
        await Task.Delay(100);
        await InvokeAsync(StateHasChanged);

        _noteTextCss = FlashCss;
        _noteText = text;
        _noteTextSeverity = severity;
        _noteTextNoIcon = !icon;
        await InvokeAsync(StateHasChanged);
    }

    protected async override Task OnInitializedAsync()
    {
        MidiService.StatusChanged += MidiStatusChanged;
        await ChangeNoteText();
    }

    private async void MidiStatusChanged(object? sender, EventArgs e) 
    {
        await ChangeNoteText();
    }

    private async Task ChangeNoteText(string? note = null) 
    {
        if (note is string text)
        {
            await ChangeNoteText(text, Severity.Info, false);
            return;
        }

        switch (MidiService.MidiStatus)
        {
            case MidiStatus.NoDevices:
                await ChangeNoteText("No MIDI devices", Severity.Error, true);
                return;
            case MidiStatus.NotSupported:
                await ChangeNoteText("MIDI not supported", Severity.Error, true);
                return;
            case MidiStatus.Error:
                await ChangeNoteText(MidiService.MidiError!, Severity.Error, true);
                return;
            case MidiStatus.Ready:
                await ChangeNoteText("Connecting", Severity.Info, true);
                return;
            case MidiStatus.Connected:
                await ChangeNoteText(MidiService.DeviceName!, Severity.Success, true);
                return;
        }
    }
}
