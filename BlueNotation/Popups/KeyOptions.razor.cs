using BlueNotation.Game;
using BlueNotation.Music;
using BlueNotation.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlueNotation.Popups;

public partial class KeyOptions
{
    [CascadingParameter] MudDialogInstance? MudDialog { get; set; }

    private bool _success;
    private string value { get; set; } = "Nothing selected";
    private IEnumerable<string> _chosenKeys { get; set; } = new HashSet<string>() { };

    private string _direction = "Both";

    private bool _retry = false;

    private bool _allowConsecutive = false;

    private string _endMode = "Timer";

    private int _endValue = 0;

    private string _clefs = "Treble";

    private readonly List<string> _allKeyNames;
    private readonly Dictionary<string, Key> _keyNamePairs;
    private readonly List<Key> _allKeys = new()
    {
        new(Letter.C, Accidental.Natural),
        new(Letter.G, Accidental.Natural),
        new(Letter.D, Accidental.Natural),
        new(Letter.A, Accidental.Natural),
        new(Letter.E, Accidental.Natural),
        new(Letter.B, Accidental.Natural),
        new(Letter.C, Accidental.Flat),
        new(Letter.F, Accidental.Sharp),
        new(Letter.G, Accidental.Flat),
        new(Letter.D, Accidental.Flat),
        new(Letter.C, Accidental.Sharp),
        new(Letter.A, Accidental.Flat),
        new(Letter.E, Accidental.Flat),
        new(Letter.B, Accidental.Flat),
        new(Letter.F, Accidental.Natural)
    };

    private string GetMultiSelectionText(List<string> selectedValues)
    {
        return $"{selectedValues.Count} key{(selectedValues.Count > 1 ? "s have" : " has")} been selected";
    }

    public KeyOptions()
    {
        _keyNamePairs = new();
        _allKeyNames = new();

        foreach (var key in _allKeys)
        {
            var name = key.ToString();
            _allKeyNames.Add(name);
            _keyNamePairs.Add(name, key);
        }
    }

    private async Task Submit()
    {
        var preset = new KeysSessionPreset();

        preset.Name = "unnamed";
        preset.TrebleNoteRange = new() { 60, 62, 64, 65, 67 };
        preset.BassNoteRange = new() { 45, 47, 48, 50};

        preset.ClefMode = ClefMode.Treble;
        if (_clefs == "Bass")
        {
            preset.ClefMode = ClefMode.Bass;
        }
        if (_clefs == "Both")
        {
            preset.ClefMode = ClefMode.Both;
        }

        preset.EndMode = EndMode.Infinite;
        if (_endMode == "Timer")
        {
            preset.EndMode = EndMode.Timer;
            preset.TimerSeconds = _endValue;
        }
        if (_endMode == "Questions")
        {
            preset.EndMode = EndMode.QuestionCount;
            preset.QuestionCount= _endValue;
        }

        preset.Direction = Game.Direction.Up;
        if (_direction == "Down")
        {
            preset.Direction = Game.Direction.Down;
        }
        if (_direction == "Both")
        {
            preset.Direction = Game.Direction.Both;
        }
        if (_direction == "Shuffle")
        {
            preset.Direction = Game.Direction.Shuffle;
        }

        preset.ForceRetry = _retry;
        preset.AllowRepeats= _allowConsecutive;

        preset.Keys = new();
        foreach (var keyName in _chosenKeys)
        {
            preset.Keys.Add(_keyNamePairs[keyName]);
        }

        ConductorService.Preset = preset;

        MudDialog?.Close(DialogResult.Ok(true));

        if (ConductorService.MusicArea != null)
        {
            await ConductorService.MusicArea.NewPreset();
        }
    }
}
