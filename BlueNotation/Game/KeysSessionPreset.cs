using BlueNotation.Music;

namespace BlueNotation.Game;

public class KeysSessionPreset : SessionPreset
{
    public List<Key> Keys { get; set; } = new() { new(Letter.C, Accidental.Natural), new(Letter.G, Accidental.Natural) };
    public Direction Direction { get; set; } = Direction.Up;
    public bool ForceRetry { get; set; } = false;

}

public enum Direction
{
    Up,
    Down,
    Both,
    Shuffle
}
