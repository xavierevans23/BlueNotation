using MudBlazor;
using BlueNotation.Services;
using System.Diagnostics;
using BlueNotation.Game;
using BlueNotation.Music;

namespace BlueNotation.Shared;

public partial class MusicArea : IDisposable
{
    private const string FlashCssA = "flashA";
    private const string FlashCssB = "flashB";
    private string _noteText = "Connecting";
    private string _noteTextCss = "";
    private Severity _noteTextSeverity = Severity.Info;
    private bool _noteTextNoIcon = false;

    private string? _topOverlay;
    private string? _bottomOverlay;

    private readonly Stopwatch _timer = new();
    private readonly Stopwatch _latencyTimer = new();
    private State _state = State.Ready;
    private SessionPreset _preset = new NotesSessionPreset();
    private ISession _session;

    private bool _showStats = false;

    private bool _showPlayButton = true;
    private bool _disableFinishButton = true;

    private string? _leftText;
    private string? _rightText;

    private Stave? _stave;

    private PeriodicTimer? _clock;

    private bool _disposed = false;

    public MusicArea()
    {
        _session = new NotesSession((NotesSessionPreset)_preset);
    }

    private async Task SetReady()
    {
        _state = State.Ready;

        if (_preset is NotesSessionPreset notePreset)
        {
            _session = new NotesSession(notePreset);
        }

        if (_preset is KeysSessionPreset keysPreset)
        {
            _session = new KeysSession(keysPreset);
        }

        _timer.Reset();
        _latencyTimer.Restart();
        _clock?.Dispose();

        await ShowOverlayText("Press any key to start", "");

        if (_stave is not null)
        {
            await _stave.SetDisplay();
        }

        _leftText = null;
        _rightText = null;
        _showPlayButton = true;
        _disableFinishButton = true;

        await InvokeAsync(StateHasChanged);
    }

    private async Task SetPlay()
    {
        _state = State.Playing;

        _timer.Start();
        _latencyTimer.Restart();
        var clockTask = StartClock();

        _showPlayButton = false;
        _disableFinishButton = false;
        await UpdateNoteDisplay();

        await ClearOverlay();
        await InvokeAsync(StateHasChanged);
        await clockTask;
    }

    private async Task SetPaused()
    {
        _state = State.Paused;

        _timer.Stop();
        _clock?.Dispose();

        await ShowOverlayText("Paused", "Press any key to continue");

        if (_stave is not null)
        {
            await _stave.SetDisplay();
        }

        _showPlayButton = true;
        _disableFinishButton = false;

        await InvokeAsync(StateHasChanged);
    }

    private async Task SetFinished()
    {
        _state = State.Finished;

        _timer.Stop();
        _clock?.Dispose();

        await ShowStatsOverlay();

        if (_stave is not null)
        {
            await _stave.SetDisplay();
        }

        _leftText = null;
        _rightText = null;
        _showPlayButton = true;
        _disableFinishButton = true;

        _session.ApplyStatistics(DataService.Statistics);
        await DataService.SaveData();

        await InvokeAsync(StateHasChanged);
    }

    private async Task PlayPausePressed()
    {
        switch (_state)
        {
            case State.Ready:
                await SetPlay();
                return;
            case State.Playing:
                await SetPaused();
                return;
            case State.Paused:
                await SetPlay();
                return;
            case State.Finished:
                await SetReady();
                return;
        }
    }

    private async Task RestartPressed()
    {
        switch (_state)
        {
            case State.Ready:
                await SetReady();
                return;
            case State.Playing:
                await SetReady();
                return;
            case State.Paused:
                await SetReady();
                return;
            case State.Finished:
                await SetReady();
                return;
        }
    }

    private async Task FinishPressed()
    {
        switch (_state)
        {
            case State.Ready:
                await SetReady();
                return;
            case State.Playing:
                await SetFinished();
                return;
            case State.Paused:
                await SetFinished();
                return;
            case State.Finished:
                return;
        }
    }

    private void PresetsPressed()
    {

    }

    private void StatisticsPressed()
    {
    }

    private void OptionsPressed()
    {
    }

    public void NewPreset(SessionPreset preset)
    {

    }

    private async Task ClockTick()
    {
        switch (_state)
        {
            case State.Ready:
                return;
            case State.Playing:
                await UpdateNoteDisplay();
                return;
            case State.Paused:
                return;
            case State.Finished:
                return;
        }
    }

    private async Task NotePlayed(int note)
    {
        switch (_state)
        {
            case State.Ready:
                await ChangeNoteText(NoteHelper.GetNote(note).ToString());
                await SetPlay();
                return;
            case State.Playing:
                _session.NotePlayed(note, (int)_latencyTimer.Elapsed.TotalMilliseconds);
                await ChangeNoteText(NoteHelper.GetNote(note).ToString());
                await UpdateNoteDisplay();
                _latencyTimer.Restart();
                return;
            case State.Paused:
                await ChangeNoteText(NoteHelper.GetNote(note).ToString());
                await SetPlay();
                return;
            case State.Finished:
                await ChangeNoteText(NoteHelper.GetNote(note).ToString());
                return;
        }
    }

    protected async override Task OnInitializedAsync()
    {
        await ChangeNoteText();
        MidiService.StatusChanged += MidiStatusChanged;
        MidiService.NoteDown += MidiNotePlayed;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SetReady();
        }
    }
    private async Task ChangeNoteText(string text, Severity severity, bool icon)
    {
        _noteTextCss = _noteTextCss == FlashCssA ? FlashCssB : FlashCssA;
        _noteText = text;
        _noteTextSeverity = severity;
        _noteTextNoIcon = !icon;
        await InvokeAsync(StateHasChanged);
    }

    private async Task ChangeNoteText(string? note = null)
    {
        if (note is string text)
        {
            await ChangeNoteText(text, Severity.Info, false);
            return;
        }

        switch (MidiService.MidiStatus)
        {
            case MidiStatus.NoDevices:
                await ChangeNoteText("No MIDI devices", Severity.Error, true);
                return;
            case MidiStatus.NotSupported:
                await ChangeNoteText("MIDI not supported", Severity.Error, true);
                return;
            case MidiStatus.Error:
                await ChangeNoteText(MidiService.MidiError!, Severity.Error, true);
                return;
            case MidiStatus.Ready:
                await ChangeNoteText("Connecting", Severity.Info, true);
                return;
            case MidiStatus.Connected:
                await ChangeNoteText(MidiService.DeviceName!, Severity.Success, true);
                return;
        }
    }

    private async Task ShowOverlayText(string top, string bottom)
    {
        if (_stave is not null)
        {
            await _stave.FadeOut();
        }

        _topOverlay = top;
        _bottomOverlay = bottom;
        _showStats = false;

        await InvokeAsync(StateHasChanged);
    }

    private async Task ShowStatsOverlay()
    {
        if (_stave is not null)
        {
            await _stave.FadeOut();
        }

        _showStats = true;
        _topOverlay = null;
        _bottomOverlay = null;

        await InvokeAsync(StateHasChanged);
    }

    private async Task ClearOverlay()
    {
        if (_stave is not null)
        {
            await _stave.FadeIn();
        }

        _showStats = false;
        _topOverlay = null;
        _bottomOverlay = null;

        await InvokeAsync(StateHasChanged);
    }

    private async Task UpdateNoteDisplay()
    {
        if (_state == State.Playing)
        {
            if (_preset.EndMode == EndMode.Timer)
            {
                if (_timer.Elapsed.TotalSeconds > _preset.TimerSeconds)
                {
                    await SetFinished();
                    return;
                }
            }
            if (_preset.EndMode == EndMode.QuestionCount)
            {
                if ((_session is NotesSession notes && notes.TotalNotesPlayed >= _preset.QuestionCount) || (
                    _session is KeysSession keys && keys.TotalScalesPlayed >= _preset.QuestionCount))
                {
                    await SetFinished();
                    return;
                }
            }
        }

        _leftText = _timer.Elapsed.ToString(@"mm\:ss");
        _rightText = _session.TotalAttempts.ToString();

        if (_stave is not null)
        {
            await _stave.SetDisplay(_session.GetNotes(), _session.UseTrebleClef, _session.Key);
        }

        await InvokeAsync(StateHasChanged);
    }

    private async void MidiNotePlayed(object? sender, int midi)
    {
        await NotePlayed(midi);
    }

    private async void MidiStatusChanged(object? sender, EventArgs e)
    {
        await ChangeNoteText();
    }

    private async Task StartClock()
    {
        _clock?.Dispose();

        _clock = new(TimeSpan.FromSeconds(1));

        while (await _clock.WaitForNextTickAsync())
        {
            await ClockTick();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                MidiService.StatusChanged -= MidiStatusChanged;
                MidiService.NoteDown -= MidiNotePlayed;
                _clock?.Dispose();
            }

            _disposed = true;
        }
    }

    ~MusicArea()
    {
        Dispose(false);
    }
}

public enum State
{
    Ready,
    Playing,
    Paused,
    Finished
}
