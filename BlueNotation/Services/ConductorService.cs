using BlueNotation.Game;
using BlueNotation.Shared;

namespace BlueNotation.Services;

public class ConductorService
{
    private readonly DataService _dataService;
    private readonly MidiService _midiService;

    private bool _started = false;

    public MusicArea? MusicArea { get; set; } = null;

    public SessionPreset Preset { get; set; }

    public ConductorService(DataService dataService, MidiService midiService)
    {
        _dataService = dataService;
        _midiService = midiService;

        Preset = new NotesSessionPreset
        {
            ClefMode = ClefMode.Both,
            EndMode = EndMode.Infinite,
            AllowRepeats = false,
            BassNoteRange = new() { 48, 50, 52, 53, 55, 57, 59, 60 },
            TrebleNoteRange = new() { 60, 62, 64, 65, 67, 69, 71, 72 },
            MaxNotes = 1,
            MinNotes = 1,
            Name = "Default",
            QuestionCount = 10,
            TimerSeconds = 30,
        };
    }

    public async Task Start()
    {
        if(_started) return;
        _started = true;

        await _dataService.LoadData();
        await _dataService.LoadPresets();

        await _midiService.StartConnection();
    }
}
