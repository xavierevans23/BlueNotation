using BlueNotation.Data;
using BlueNotation.Game;
using BlueNotation.Music;
using BlueNotation.Services;

namespace BlueNotationTests.Services;

public class DataServiceTest
{
    [Fact]
    public void DataSerializationTest()
    {
        var preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { 60, 62 },
            ClefMode = ClefMode.Treble,
            AllowRepeats = false,
            MaxNotes = 1,
            MinNotes = 1
        };

        var session = new NotesSession(preset);
        var note = session.GetNotes().First();

        session.NotePlayed(NoteHelper.GetMidi(note), 100);
        session.NotePlayed(NoteHelper.GetMidi(note), 100);

        note = session.GetNotes().First();
        session.NotePlayed(NoteHelper.GetMidi(note), 200);

        note = session.GetNotes().First();
        session.NotePlayed(NoteHelper.GetMidi(note), 50);

        var dataService = new DataService(null!);

        var stats = dataService.Statistics;
        stats.TotalNotesAttempted = 2;
        stats.TotalNotesPlayed = 1;

        session.ApplyStatistics(stats);

        var xml = dataService.SerializeData();
        dataService.DeserializeData(xml);
        stats = dataService.Statistics;

        var firstMidi = NoteHelper.GetMidi(note);
        Assert.Equal(6, stats.TotalNotesAttempted);
        Assert.Equal(4, stats.TotalNotesPlayed);

        Assert.Equal(2, stats.GetNote(firstMidi).TotalAttempts);
        Assert.Equal(2, stats.GetNote(firstMidi).TotalTimesPlayed);
        Assert.Equal(150, stats.GetNote(firstMidi).TotalLatency);

        firstMidi = firstMidi == 60 ? 62 : 60;

        Assert.Equal(2, stats.GetNote(firstMidi).TotalAttempts);
        Assert.Equal(1, stats.GetNote(firstMidi).TotalTimesPlayed);
        Assert.Equal(200, stats.GetNote(firstMidi).TotalLatency);
    }

    [Fact]
    public void PresetsSerializationTests()
    {
        var preset = new NotesSessionPreset { Name = "testname", MaxNotes = 15 };
        
        var dataService = new DataService(null!);

        dataService.PresetsData.AddNotesPreset(preset);

        var xml = dataService.SerializePresets();
        dataService.DeserializePresets(xml);

        Assert.Equal(15, (dataService.PresetsData.GetNotesPreset("testname") as NotesSessionPreset)!.MaxNotes);
    }
}
