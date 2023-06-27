using BlueNotation.Data;
using BlueNotation.Music;
using BlueNotation.Services;

namespace BlueNotation.Pages;

public partial class StatisticsPage
{
    private int _totalNotesPlayed = 0;
    private int _totalCorrect = 0;
    private int _daysSinceCreated = 0;
    private int _allTimeAccuracy = 0;

    private readonly List<TableNoteData> _noteTrebleData = new();
    private readonly List<TableNoteData> _noteBassData = new();
    private readonly List<TableKeyData> _keyData = new();

    private async Task UpdateData(int sessionCount)
    {
        var stats = DataService.Statistics;

        _totalNotesPlayed = stats.TotalNotesAttempted;
        _totalCorrect = stats.TotalNotesPlayed;
        _daysSinceCreated = (int)(DateTime.Now - stats.TimeCreated).TotalDays;

        if (_totalNotesPlayed > 0)
        {
            _allTimeAccuracy = (100 * _totalCorrect) / _totalNotesPlayed;
        }
        else
        {
            _allTimeAccuracy = 0;
        }

        GenerateNoteData(stats.TrebleNotes, _noteTrebleData, sessionCount);
        GenerateNoteData(stats.BassNotes, _noteBassData, sessionCount);
        GenerateNoteData(stats.Keys, _keyData, sessionCount);

        await InvokeAsync(StateHasChanged);
    }

    private void GenerateNoteData(List<KeyStatisticsItem> notes, List<TableKeyData> target, int _timeFrameSelection)
    {
        target.Clear();
        foreach (var noteData in notes)
        {

            var name = noteData.Key.ToString();
            var totalAttempts = 0;
            var totalCorrect = 0;
            var totalLatency = 0;
            var accuracy = 0;
            float averageTime;

            if (_timeFrameSelection == -1)
            {
                totalAttempts = noteData.TotalAttempts;
                totalCorrect = noteData.TotalTimesPlayed;
                totalLatency = noteData.TotalLatency;
            }
            else
            {
                if (_timeFrameSelection > StatisticsItem.MaxHistory)
                {
                    _timeFrameSelection = StatisticsItem.MaxHistory;
                }

                if (_timeFrameSelection < 1)
                {
                    _timeFrameSelection = 1;
                }

                var historyData = noteData.GetHistory(_timeFrameSelection - 1);

                totalAttempts = historyData.Attempts;
                totalCorrect = historyData.TimesPlayed;
                totalLatency = historyData.Latency;
            }

            if (totalAttempts > 0)
            {
                accuracy = (totalCorrect * 100) / totalAttempts;
            }

            averageTime = (totalLatency / (totalCorrect * 10)) / 100f;

            var dataTableItem = new TableKeyData
            {
                Accuracy = accuracy,
                AverageTime = averageTime,
                Key = name,
                TotalAttempts = totalAttempts,
                TotalCorrect = totalCorrect,
            };

            target.Add(dataTableItem);
        }
    }

    private void GenerateNoteData(List<NoteStatisticsItem> notes, List<TableNoteData> target, int _timeFrameSelection)
    {
        target.Clear();
        foreach (var noteData in notes)
        {

            var name = NoteHelper.GetNote(noteData.Midi);
            var totalAttempts = 0;
            var totalCorrect = 0;
            var totalLatency = 0;
            var accuracy = 0;
            float averageTime;

            if (_timeFrameSelection == -1)
            {
                totalAttempts = noteData.TotalAttempts;
                totalCorrect = noteData.TotalTimesPlayed;
                totalLatency = noteData.TotalLatency;
            }
            else
            {
                if (_timeFrameSelection > StatisticsItem.MaxHistory)
                {
                    _timeFrameSelection = StatisticsItem.MaxHistory;
                }

                if (_timeFrameSelection < 1)
                {
                    _timeFrameSelection = 1;
                }

                var historyData = noteData.GetHistory(_timeFrameSelection - 1);

                totalAttempts = historyData.Attempts;
                totalCorrect = historyData.TimesPlayed;
                totalLatency = historyData.Latency;
            }

            if (totalAttempts > 0)
            {
                accuracy = (totalCorrect * 100) / totalAttempts;
            }

            averageTime = (totalLatency / (totalCorrect * 10)) / 100f;

            var dataTableItem = new TableNoteData
            {
                Accuracy = accuracy,
                AverageTime = averageTime,
                Note = name,
                TotalAttempts = totalAttempts,
                TotalCorrect = totalCorrect,
            };

            target.Add(dataTableItem);
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await UpdateData(-1);
    }
}
