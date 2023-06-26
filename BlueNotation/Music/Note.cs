namespace BlueNotation.Music;

public readonly record struct Note(Letter Letter, int Octave, Accidental Accidental)
{
    public override string ToString()
    {
        return $"{Letter}{Octave} {Accidentals[Accidental]}";
    }

    private static Dictionary<Accidental, string> Accidentals = new Dictionary<Accidental, string>
    {
        { Accidental.Natural, ""},
        { Accidental.Sharp, "♯"},
        { Accidental.Flat, "♭"},
    };
}
