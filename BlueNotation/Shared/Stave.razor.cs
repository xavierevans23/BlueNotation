using BlueNotation.Music;
using Microsoft.JSInterop;

namespace BlueNotation.Shared;

public partial class Stave
{
    private readonly string _divId;

    private static readonly Dictionary<Accidental, string> AccidentalNames = new()
    {
        { Accidental.Natural, "n"},
        { Accidental.Sharp, "#"},
        { Accidental.Flat, "b"}
    };

    public Stave()
    {
        _divId = Guid.NewGuid().ToString();
    }

    public async Task SetDisplay(IEnumerable<Note>? notes = null, bool useTrebleClef = true, Key? key = null)
    {
        key ??= new(Letter.C, Accidental.Natural);

        if (notes is null || !notes.Any())
        {
            notes = new List<Note>();
        }
        var notesList = notes.ToList();        
        
        notesList.Sort((a, b) => NoteHelper.GetMidi(a) - NoteHelper.GetMidi(b));       

        var noteNames = new List<string>();
        foreach (var note in notesList)
        {
            noteNames.Add($"{note.Letter}/{note.Octave}");
        }

        var keyString = $"{key.Value.Letter}{(key.Value.Accidental == Accidental.Natural ? "" : AccidentalNames[key.Value.Accidental])}";

        var beatCount = notes.Any() ? 1 : 0;

        await JS.InvokeVoidAsync("draw", _divId, noteNames, "q", beatCount, 4, "4/4", keyString, useTrebleClef ? "treble" : "bass");
    }
}
