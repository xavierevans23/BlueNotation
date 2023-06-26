using BlueNotation.Music;

namespace BlueNotation.Game;

public class KeysSessionPreset : SessionPreset
{
    public List<Key> Keys { get; set; } = new();
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
