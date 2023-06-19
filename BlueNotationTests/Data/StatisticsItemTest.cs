using BlueNotation.Data;

namespace BlueNotationTests.Data;

public class StatisticsItemTest
{
    [Fact]
    public async Task TestAddData()
    {
        var noteItem = new NoteStatisticsItem();
        await Task.Delay(100);

        noteItem.AddData(10, 7, 700);
        Assert.Equal(10, noteItem.TotalAttempts);
        Assert.Equal(7, noteItem.TotalTimesPlayed);
        Assert.Equal(700, noteItem.TotalLatency);
        Assert.True((DateTime.Now - noteItem.LastPlayed).TotalMilliseconds < 90);

        noteItem.AddData(15, 2, 5);
        Assert.Equal(25, noteItem.TotalAttempts);
        Assert.Equal(9, noteItem.TotalTimesPlayed);
        Assert.Equal(705, noteItem.TotalLatency);

        Assert.Equal(2, noteItem.HistoryPointer);
    }

    [Fact]
    public async Task TestGetHistory()
    {
        var noteItem = new NoteStatisticsItem();
        await Task.Delay(100);

        noteItem.AddData(10, 7, 700);
        noteItem.AddData(15, 2, 350);

        var history = noteItem.GetHistory(1);
        Assert.Equal(15, history.Attempts);
        Assert.Equal(2, history.TimesPlayed);
        Assert.Equal(350, history.Latency);

        history = noteItem.GetHistory(3);
        Assert.Equal(25, history.Attempts);
        Assert.Equal(9, history.TimesPlayed);
        Assert.Equal(1050, history.Latency);
    }
}
