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
        throw new NotImplementedException();
    }

    public static int GetMidi(Note note)
    {
        throw new NotImplementedException();
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
        Accidental.Natural,
    };
}
