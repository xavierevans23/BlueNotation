namespace BlueNotation.Game;

public class NotesSessionPreset : SessionPreset
{
    public List<int> TrebleNoteRange { get; set; } = new() { 60,62,64,65,67,69,71,72};
    public List<int> BassNoteRange { get; set; } = new() { 60,62,64,65,67,69,71,72};
    public int MaxNotes { get; set; } = 1;
    public int MinNotes { get; set; } = 1;
}