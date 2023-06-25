namespace BlueNotation.Services;

public class ConductorService
{
    private readonly DataService _dataService;
    private readonly MidiService _midiService;

    private bool _started = false;

    public ConductorService(DataService dataService, MidiService midiService)
    {
        _dataService = dataService;
        _midiService = midiService;
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
