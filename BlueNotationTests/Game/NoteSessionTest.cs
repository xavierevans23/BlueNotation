using BlueNotation.Data;
using BlueNotation.Game;
using BlueNotation.Music;

namespace BlueNotationTests.Game;

public class NoteSessionTest
{
    [Fact]
    public void TestRange()
    {
        var preset = new NotesSessionPreset();
        preset.TrebleNoteRange = new() { 60, 62 };
        preset.CleffMode = CleffMode.Treble;
        preset.AllowRepeats = false;
        preset.MaxNotes = 2;
        preset.MinNotes = 2;

        var session = new NotesSession(preset);
        var notes = session.GetNotes().Select(NoteHelper.GetMidi).ToList();

        Assert.Equal(2, notes.Count());
        Assert.Contains(60, notes);
        Assert.Contains(62, notes);
    }

    [Fact]
    public void TestRepeat()
    {
        var preset = new NotesSessionPreset();
        preset.TrebleNoteRange = new() { 60, 62 };
        preset.CleffMode = CleffMode.Treble;
        preset.AllowRepeats = false;
        preset.MaxNotes = 1;
        preset.MinNotes = 1;

        var session = new NotesSession(preset);
        var first = session.GetNotes().First();

        session.NotePlayed(NoteHelper.GetMidi(first), 100);
        Assert.False(first == session.GetNotes().First());

        first = session.GetNotes().First();

        session.NotePlayed(NoteHelper.GetMidi(first), 100);
        Assert.False(first == session.GetNotes().First());

        first = session.GetNotes().First();

        session.NotePlayed(NoteHelper.GetMidi(first), 100);
        Assert.False(first == session.GetNotes().First());

        first = session.GetNotes().First();

        session.NotePlayed(NoteHelper.GetMidi(first), 100);
        Assert.False(first == session.GetNotes().First());
    }

    [Fact]
    public void TestNoteCount()
    {
        var preset = new NotesSessionPreset();
        preset.TrebleNoteRange = new() { 60, 62 };
        preset.CleffMode = CleffMode.Treble;
        preset.AllowRepeats = false;
        preset.MaxNotes = 5;
        preset.MinNotes = 5;

        var session = new NotesSession(preset);
        Assert.Equal(2, session.GetNotes().Count());

        preset = new NotesSessionPreset();
        preset.TrebleNoteRange = new() { 60, 62, 64, 65, 67 };
        preset.CleffMode = CleffMode.Treble;
        preset.AllowRepeats = false;
        preset.MaxNotes = 5;
        preset.MinNotes = 5;

        session = new NotesSession(preset);
        Assert.Equal(5, session.GetNotes().Count());
    }

    [Fact]
    public void TestNoteCleff()
    {
        var preset = new NotesSessionPreset();
        preset.TrebleNoteRange = new() { 60, 62 };
        preset.CleffMode = CleffMode.Treble;
        preset.AllowRepeats = false;
        preset.MaxNotes = 5;
        preset.MinNotes = 5;

        var session = new NotesSession(preset);
        Assert.True(session.UseTrebleCleff);

        preset = new NotesSessionPreset();
        preset.TrebleNoteRange = new() { 60, 62 };
        preset.CleffMode = CleffMode.Bass;
        preset.AllowRepeats = false;
        preset.MaxNotes = 5;
        preset.MinNotes = 5;

        session = new NotesSession(preset);
        Assert.False(session.UseTrebleCleff);
    }

    [Fact]
    public void TestNotePlayed()
    {
        var preset = new NotesSessionPreset();
        preset.TrebleNoteRange = new() { 60, 62 };
        preset.CleffMode = CleffMode.Treble;
        preset.AllowRepeats = false;
        preset.MaxNotes = 1;
        preset.MinNotes = 1;

        var session = new NotesSession(preset);
        var note = session.GetNotes().First();

        Assert.True(session.NotePlayed(NoteHelper.GetMidi(note), 100));
        Assert.False(session.NotePlayed(NoteHelper.GetMidi(note), 100));

        note = session.GetNotes().First();
        Assert.True(session.NotePlayed(NoteHelper.GetMidi(note), 100));
    }

    [Fact]
    public void TestKey()
    {
        Assert.Equal(new Key(Letter.C, Accidental.Natural), new NotesSession(new()).Key);
    }

    [Fact]
    public void TestStatistics()
    {
        var preset = new NotesSessionPreset();
        preset.TrebleNoteRange = new() { 60, 62 };
        preset.CleffMode = CleffMode.Treble;
        preset.AllowRepeats = false;
        preset.MaxNotes = 1;
        preset.MinNotes = 1;

        var session = new NotesSession(preset);
        var note = session.GetNotes().First();

        Assert.True(session.NotePlayed(NoteHelper.GetMidi(note), 100));
        Assert.False(session.NotePlayed(NoteHelper.GetMidi(note), 100));

        note = session.GetNotes().First();
        Assert.True(session.NotePlayed(NoteHelper.GetMidi(note), 200));

        note = session.GetNotes().First();
        Assert.True(session.NotePlayed(NoteHelper.GetMidi(note), 50));

        Assert.Equal(3, session.TotalNotesPlayed);
        Assert.Equal(4, session.TotalAttempts);

        var stats = new Statistics() { TotalNotesAttempted =2, TotalNotesPlayed = 1};
        session.ApplyStatistics(stats);

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
}
