using BlueNotation.Data;
using BlueNotation.Game;
using BlueNotation.Music;

namespace BlueNotationTests.Game;

public class NoteSessionTest
{
    [Fact]
    public void TestRange()
    {
        var preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { 60, 62 },
            CleffMode = CleffMode.Treble,
            AllowRepeats = false,
            MaxNotes = 2,
            MinNotes = 2
        };

        var session = new NotesSession(preset);
        var notes = session.GetNotes().Select(NoteHelper.GetMidi).ToList();

        Assert.Equal(2, notes.Count);
        Assert.Contains(60, notes);
        Assert.Contains(62, notes);

        preset = new NotesSessionPreset
        {
            BassNoteRange = new() { 60, 62 },
            CleffMode = CleffMode.Bass,
            AllowRepeats = false,
            MaxNotes = 2,
            MinNotes = 2
        };

        session = new NotesSession(preset);
        notes = session.GetNotes().Select(NoteHelper.GetMidi).ToList();

        Assert.Equal(2, notes.Count);
        Assert.Contains(60, notes);
        Assert.Contains(62, notes);
    }

    [Fact]
    public void TestRepeat()
    {
        var preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { 60, 62 },
            CleffMode = CleffMode.Treble,
            AllowRepeats = false,
            MaxNotes = 1,
            MinNotes = 1
        };

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
        var preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { 60, 62 },
            CleffMode = CleffMode.Treble,
            AllowRepeats = false,
            MaxNotes = 5,
            MinNotes = 5
        };

        var session = new NotesSession(preset);
        Assert.Equal(2, session.GetNotes().Count());

        preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { 60, 62, 64, 65, 67 },
            CleffMode = CleffMode.Treble,
            AllowRepeats = false,
            MaxNotes = 5,
            MinNotes = 5
        };

        session = new NotesSession(preset);
        Assert.Equal(5, session.GetNotes().Count());
    }

    [Fact]
    public void TestNoteCleff()
    {
        var preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { 60, 62 },
            CleffMode = CleffMode.Treble,
            AllowRepeats = false,
            MaxNotes = 5,
            MinNotes = 5
        };

        var session = new NotesSession(preset);
        Assert.True(session.UseTrebleCleff);

        preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { 60, 62 },
            CleffMode = CleffMode.Bass,
            AllowRepeats = false,
            MaxNotes = 5,
            MinNotes = 5
        };

        session = new NotesSession(preset);
        Assert.False(session.UseTrebleCleff);
    }

    [Fact]
    public void TestNotePlayed()
    {
        var preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { 60, 62 },
            CleffMode = CleffMode.Treble,
            AllowRepeats = false,
            MaxNotes = 1,
            MinNotes = 1
        };

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
        var preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { 60, 62 },
            CleffMode = CleffMode.Treble,
            AllowRepeats = false,
            MaxNotes = 1,
            MinNotes = 1
        };

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

        var history = session.GetNoteStatistics().ToDictionary(a => a.Key, b => b.Value);

        Assert.True(history.ContainsKey(note));
        Assert.Equal(2, history.Count);

        var stats = new Statistics() { TotalNotesAttempted = 2, TotalNotesPlayed = 1 };

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

    [Fact]
    public void TestValidation()
    {
        var preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { 60, 62 },
            CleffMode = CleffMode.Treble,
            AllowRepeats = false,
            MaxNotes = 1,
            MinNotes = 0
        };

        Assert.Throws<ArgumentException>(() => new NotesSession(preset));

        preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { 60, 62 },
            CleffMode = CleffMode.Treble,
            AllowRepeats = false,
            MaxNotes = 0,
            MinNotes = 1
        };

        Assert.Throws<ArgumentException>(() => new NotesSession(preset));

        preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { 60, 62 },
            CleffMode = CleffMode.Treble,
            AllowRepeats = false,
            MaxNotes = 5,
            MinNotes = 6
        };

        Assert.Throws<ArgumentException>(() => new NotesSession(preset));

        preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { },
            CleffMode = CleffMode.Treble,
            AllowRepeats = false,
            MaxNotes = 1,
            MinNotes = 1
        };

        Assert.Throws<ArgumentException>(() => new NotesSession(preset));

        preset = new NotesSessionPreset
        {
            BassNoteRange = new() { },
            CleffMode = CleffMode.Treble,
            AllowRepeats = false,
            MaxNotes = 1,
            MinNotes = 1
        };

        Assert.Throws<ArgumentException>(() => new NotesSession(preset));
    }
}
