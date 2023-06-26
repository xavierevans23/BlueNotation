using BlueNotation.Shared;

namespace BlueNotation.Music;

public readonly record struct Key(Letter Letter, Accidental Accidental)
{
    public override string ToString()
    {
        return $"{Letter} {Accidentals[Accidental]}";
    }

    private readonly static Dictionary<Accidental, string> Accidentals = new()
    {
        { Accidental.Natural, ""},
        { Accidental.Sharp, "♯"},
        { Accidental.Flat, "♭"},
    };
}
