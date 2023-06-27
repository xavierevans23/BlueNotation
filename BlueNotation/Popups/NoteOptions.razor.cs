using BlueNotation.Game;
using BlueNotation.Music;
using BlueNotation.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlueNotation.Popups;

public partial class NoteOptions : IDisposable
{
    [CascadingParameter] MudDialogInstance? MudDialog { get; set; }

    private bool _disposed;

    private string _trebleNotes = "";
    private string _bassNotes = "";
    private string _minNotes = "";
    private string _maxNotes = "";

    private string _lastPlayed = "";

    private bool _success;
    
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

    protected override void OnParametersSet()
    {
        MidiService.NoteDown += MidiNotePlayed;
    }

    public NoteOptions()
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

    public static List<int> ParseNumbers(string input)
    {
        List<int> numbers = new List<int>();

        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input string cannot be null or empty.");

        string[] tokens = input.Split(',');

        foreach (string token in tokens)
        {
            string trimmedToken = token.Trim();

            if (trimmedToken.Contains("-"))
            {
                string[] rangeTokens = trimmedToken.Split('-');
                if (rangeTokens.Length != 2)
                    throw new ArgumentException("Invalid range format.");

                if (int.TryParse(rangeTokens[0], out int start) && int.TryParse(rangeTokens[1], out int end))
                {
                    if (start > end)
                        throw new ArgumentException("Invalid range.");

                    numbers.AddRange(Enumerable.Range(start, end - start + 1));
                }
                else
                {
                    throw new ArgumentException("Invalid number in range.");
                }
            }
            else
            {
                if (int.TryParse(trimmedToken, out int number))
                {
                    numbers.Add(number);
                }
                else
                {
                    throw new ArgumentException("Invalid number format.");
                }
            }
        }

        numbers = numbers.Distinct().Where(n => NoteHelper.GetNote(n).Accidental == Accidental.Natural).ToList();

        if (!numbers.Any())
        {
            throw new ArgumentException("Must be at least one number.");
        }

        return numbers;
    }

    private string? _error;

    private async Task Submit()
    {
        var preset = new NotesSessionPreset();

        _error = null;

        preset.Name = "unnamed";
        try
        {
            preset.TrebleNoteRange = ParseNumbers(_trebleNotes);
            preset.BassNoteRange = ParseNumbers(_bassNotes);
        }
        catch (ArgumentException e)
        {
            _error = e.Message;
            return;
        }

        preset.MinNotes = int.Parse(_minNotes);
        preset.MaxNotes = int.Parse(_maxNotes);

        if (preset.MinNotes > preset.MaxNotes)
        {
            var t = preset.MinNotes;
            preset.MinNotes = preset.MaxNotes;
            preset.MaxNotes = t;
        }

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

        ConductorService.Preset = preset;

        MudDialog?.Close(DialogResult.Ok(true));

        if (ConductorService.MusicArea != null)
        {
            await ConductorService.MusicArea.NewPreset();
        }
    }

    private async void MidiNotePlayed(object? sender, int note)
    {
        _lastPlayed = note.ToString();
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                MidiService.NoteDown -= MidiNotePlayed;
            }

            _disposed = true;
        }
    }

    ~NoteOptions()
    {
        Dispose(false);
    }
}
