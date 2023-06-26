using BlueNotation.Data;
using BlueNotation.Music;

namespace BlueNotationTests.Data;

public class StatisticsTest
{
    [Fact]
    public void TestUnloadData()
    {
        var stats = new Statistics();

        stats.TrebleNotes.Add(new() { Midi = 3, TotalAttempts = 3 });
        stats.TrebleNotes.Add(new() { Midi = 4, TotalAttempts = 4 });

        stats.Keys.Add(new() { Key = new(Letter.C, Accidental.Natural), TotalAttempts = 3 });
        stats.Keys.Add(new() { Key = new(Letter.D, Accidental.Natural), TotalAttempts = 4 });

        stats.UnloadData();

        Assert.Equal(3, stats.GetTrebleNote(3).TotalAttempts);
        Assert.Equal(4, stats.GetTrebleNote(4).TotalAttempts);

        Assert.Equal(3, stats.GetKey(new(Letter.C, Accidental.Natural)).TotalAttempts);
        Assert.Equal(4, stats.GetKey(new(Letter.D, Accidental.Natural)).TotalAttempts);
    }

    [Fact]
    public void TestLoadData()
    {
        var stats = new Statistics();

        stats.AddTrebleNote(new() { Midi = 3, TotalAttempts = 3 });
        stats.AddTrebleNote(new() { Midi = 4, TotalAttempts = 4 });

        stats.AddKey(new() { Key = new(Letter.C, Accidental.Natural), TotalAttempts = 3 });
        stats.AddKey(new() { Key = new(Letter.D, Accidental.Natural), TotalAttempts = 4 });

        stats.LoadData();

        Assert.Contains(stats.TrebleNotes, n => n.Midi == 3 && n.TotalAttempts == 3);
        Assert.Contains(stats.TrebleNotes, n => n.Midi == 4 && n.TotalAttempts == 4);

        Assert.Contains(stats.Keys, k => k.Key == new Key(Letter.C, Accidental.Natural) && k.TotalAttempts == 3);
        Assert.Contains(stats.Keys, k => k.Key == new Key(Letter.D, Accidental.Natural) && k.TotalAttempts == 4);
    }

    [Fact]
    public void TestAddNote()
    {
        var stats = new Statistics();

        stats.AddTrebleNote(new() { Midi = 3, TotalAttempts = 3 });
        Assert.Equal(3, stats.GetTrebleNote(3).TotalAttempts);

        stats.AddTrebleNote(new() { Midi = 3, TotalAttempts = 5 });
        Assert.Equal(5, stats.GetTrebleNote(3).TotalAttempts);
    }

    [Fact]
    public void TestAddKey()
    {
        var stats = new Statistics();

        stats.AddKey(new() { Key = new(Letter.C, Accidental.Natural), TotalAttempts = 3 });
        Assert.Equal(3, stats.GetKey(new(Letter.C, Accidental.Natural)).TotalAttempts);

        stats.AddKey(new() { Key = new(Letter.C, Accidental.Natural), TotalAttempts = 5 });
        Assert.Equal(5, stats.GetKey(new(Letter.C, Accidental.Natural)).TotalAttempts);
    }

    [Fact]
    public void TestGetNote()
    {
        var stats = new Statistics();

        Assert.Equal(0, stats.GetTrebleNote(3).TotalAttempts);
        
        stats.AddTrebleNote(new() { Midi = 3, TotalAttempts = 3 });
        Assert.Equal(3, stats.GetTrebleNote(3).TotalAttempts);
    }

    [Fact]
    public void TestGetKey()
    {
        var stats = new Statistics();

        Assert.Equal(0, stats.GetKey(new(Letter.C, Accidental.Natural)).TotalAttempts);
        
        stats.AddKey(new() { Key = new(Letter.C, Accidental.Natural), TotalAttempts = 3 });
        Assert.Equal(3, stats.GetKey(new(Letter.C, Accidental.Natural)).TotalAttempts);
    }
}
