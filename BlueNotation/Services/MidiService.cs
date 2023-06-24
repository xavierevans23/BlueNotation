using Microsoft.JSInterop;

namespace BlueNotation.Services;

public class MidiService
{
    private readonly IJSRuntime _jsRuntime;    
    private bool _started = false;
    
    private static MidiService? _instance;

    public MidiStatus MidiStatus { get; private set; } = MidiStatus.Ready;
    public string? DeviceName { get; private set; }
    public string? MidiError { get; private set; }

    public event EventHandler<int>? NoteDown;
    public event EventHandler? StatusChanged;    

    public MidiService(IJSRuntime jsRuntime) 
    {
        _jsRuntime = jsRuntime;
        _instance = this;
    }

    public async Task StartConnection()
    {
        if (_started) return;

        _started = true;
        await _jsRuntime.InvokeVoidAsync("midiStart");
    }

    [JSInvokable]
    public static void MidiEvent(string midiStatus, string deviceName, string midiError)
    {
        if (_instance is null) return;

        _instance.DeviceName = null;
        _instance.MidiError = null;

        switch (midiStatus)
        {
            case "ready":
                _instance.MidiStatus = MidiStatus.Ready;                
                break;
            case "unsupported":
                    _instance.MidiStatus = MidiStatus.NotSupported;
                break;
            case "error":
                _instance.MidiStatus = MidiStatus.Error;
                _instance.MidiError = midiError;
                break;
            case "noDevices":
                _instance.MidiStatus = MidiStatus.NoDevices;
                break;
            case "connected":
                _instance.MidiStatus = MidiStatus.Connected;
                _instance.DeviceName = deviceName;
                break;
            default:
                _instance.MidiStatus = MidiStatus.Error;
                _instance.MidiError = "Unknown error.";
                break;
        }

        var statusChanged = _instance.StatusChanged;
        
        if (statusChanged is not null)
        {
            statusChanged(_instance, EventArgs.Empty);
        }
    }

    [JSInvokable]
    public static void NotePlayed(int note)
    {
        if (_instance is null)
        {
            return;
        }

        var noteDown = _instance.NoteDown;

        if (noteDown is not null)
        {
            noteDown(_instance, note);
        }
    }
}

public enum MidiStatus
{
    Ready,
    Error,
    NotSupported,
    NoDevices,
    Connected
}
