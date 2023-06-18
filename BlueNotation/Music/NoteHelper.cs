namespace BlueNotation.Music;

public static class NoteHelper
{
    public static Note GetNote(int midi)
    {
        var offset = midi % 12;

        if (offset < 0)
        {
            offset += 12;
        }

        var octave = ((midi - offset) / 12) - 1;

        return new(_offsetLetters[offset], octave, _offsetAccidentals[offset]);
    }

    public static Note GetNote(int midi, Key key)
    {
        var offset = midi % 12;

        if (offset < 0)
        {
            offset += 12;
        }

        if (!_scaleNotes.ContainsKey(key))
        {
            throw new ArgumentException($"There's no major key \"{key}\".");
        }

        if (!_scaleNotes[key].ContainsKey(offset))
        {
            return GetNote(midi);
        }
        
        var octave = ((midi - offset) / 12) - 1;

        var note = _scaleNotes[key][offset];

        note = note with { Octave = octave + note.Octave };

        return note;
    }

    public static int GetMidi(Note note)
    {
        return _letterOffsets[note.Letter] + ((note.Octave + 1) * 12) + _accidentalOffsets[note.Accidental];
    }

    public static IEnumerable<int> GetScale(Letter startLetter)
    {
        throw new NotImplementedException();
    }

    private static readonly Letter[] _offsetLetters = new[] {
        Letter.C,
        Letter.C,
        Letter.D,
        Letter.E,
        Letter.E,
        Letter.F,
        Letter.F,
        Letter.G,
        Letter.A,
        Letter.A,
        Letter.B,
        Letter.B
    };

    private static readonly Accidental[] _offsetAccidentals = new[] {
        Accidental.Natural,
        Accidental.Sharp,
        Accidental.Natural,
        Accidental.Flat,
        Accidental.Natural,
        Accidental.Natural,
        Accidental.Sharp,
        Accidental.Natural,
        Accidental.Flat,
        Accidental.Natural,
        Accidental.Flat,
        Accidental.Natural
    };

    private static readonly Dictionary<Letter, int> _letterOffsets = new() {
        { Letter.C, 0},
        { Letter.D, 2},
        { Letter.E, 4},
        { Letter.F, 5},
        { Letter.G, 7},
        { Letter.A, 9},
        { Letter.B, 11}
    };

    private static readonly Dictionary<Accidental, int> _accidentalOffsets = new() {
        { Accidental.Natural, 0},
        { Accidental.Sharp, 1},
        { Accidental.Flat, -1}
    };

    static NoteHelper()
    {
        foreach (var scale in _majorScaleData)
        {
            var keyStrings = scale.Split(' ');
            var keys = new List<Key>();

            foreach (var key in keyStrings)
            {
                keys.Add(new(_letterChars[key[0]], _accidentalChars[key[1]]));
            }

            var first = keys.First();
            var midiStart = GetMidi(new(first.Letter, 0, first.Accidental));

            _scaleNotes.Add(first, new());

            for (int i = 0; i < 7; i++)
            {
                var midi = midiStart + _majorScaleSteps[i];

                var offset = midi % 12;

                if (offset < 0)
                {
                    offset += 12;
                }

                var key = keys[i];
                var octave = 0;

                if (key.Letter == Letter.C && key.Accidental == Accidental.Flat)
                {
                    octave++;
                }
                if (key.Letter == Letter.B && key.Accidental == Accidental.Sharp)
                {
                    octave--;
                }

                var note = new Note(key.Letter, octave, key.Accidental);

                _scaleNotes[first][offset] = note;
            }
        }
    }

    private static readonly Dictionary<Key, Dictionary<int, Note>> _scaleNotes = new();

    private static readonly int[] _majorScaleSteps = new[] { 0, 2, 4, 5, 7, 9, 11 };

    private static readonly string[] _majorScaleData = new[]
    {
        "Cn Dn En Fn Gn An Bn",
        "Gn An Bn Cn Dn En F#",
        "Dn En F# Gn An Bn C#",
        "An Bn C# Dn En F# G#",
        "En F# G# An Bn C# D#",
        "Fn Gn An Bb Cn Dn En",
        "Bb Cn Dn Eb Fn Gn An",
        "Eb Fn Gn Ab Bb Cn Dn",
        "Ab Bb Cn Db Eb Fn Gn",
        "Bn C# D# En F# G# A#",
        "Cb Db Eb Fb Gb Ab Bb",
        "F# G# A# Bn C# D# E#",
        "Gb Ab Bb Cb Db Eb Fn",
        "C# D# E# F# G# A# B#",
        "Db Eb Fn Gb Ab Bb Cn",
    };

    private static readonly Dictionary<char, Letter> _letterChars = new() {
        { 'C', Letter.C},
        { 'D', Letter.D},
        { 'E', Letter.E},
        { 'F', Letter.F},
        { 'G', Letter.G},
        { 'A', Letter.A},
        { 'B', Letter.B}
    };

    private static readonly Dictionary<char, Accidental> _accidentalChars = new() {
        { 'n', Accidental.Natural},
        { '#', Accidental.Sharp},
        { 'b', Accidental.Flat}
    };
}
