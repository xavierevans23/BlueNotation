using BlueNotation.Data;
using BlueNotation.Game;
using BlueNotation.Music;
using System.Collections.Generic;

namespace BlueNotationTests.Game;

public class KeySessionTest
{
    [Fact]
    public void TestSelectKey()
    {
        var preset = new KeysSessionPreset
        {
            AllowRepeats = false,
            BassNoteRange = new List<int> { 60 },
            TrebleNoteRange = new List<int> { 61 },
            CleffMode = CleffMode.Treble,
            Direction = Direction.Up,
            ForceRetry = false,
            Keys = new List<Key> { new(Letter.D, Accidental.Flat) }
        };

        var session = new KeysSession(preset);

        Assert.Equal(new Key(Letter.D, Accidental.Flat), session.Key);
        Assert.Equal(new Note(Letter.D, 4, Accidental.Flat), session.GetNotes().First());

        preset.CleffMode = CleffMode.Bass;
        session = new KeysSession(preset);

        Assert.Equal(new Key(Letter.D, Accidental.Flat), session.Key);
        Assert.Equal(new Note(Letter.C, 4, Accidental.Natural), session.GetNotes().First());
    }

    [Fact]
    public void TestDirection()
    {
        var preset = new KeysSessionPreset
        {
            AllowRepeats = false,
            BassNoteRange = new List<int> { 60 },
            TrebleNoteRange = new List<int> { 61 },
            CleffMode = CleffMode.Treble,
            Direction = Direction.Up,
            ForceRetry = false,
            Keys = new List<Key> { new(Letter.D, Accidental.Flat) }
        };

        var session = new KeysSession(preset);

        var first = NoteHelper.GetMidi(session.GetNotes().First());

        session.NotePlayed(first, 100);

        var second = NoteHelper.GetMidi(session.GetNotes().First());

        Assert.True(first < second);

        preset.Direction = Direction.Down;

        session = new KeysSession(preset);

        first = NoteHelper.GetMidi(session.GetNotes().First());

        session.NotePlayed(first, 100);

        second = NoteHelper.GetMidi(session.GetNotes().First());

        Assert.True(first > second);
    }

    [Fact]
    public void TestAllowRepeat()
    {
        var preset = new KeysSessionPreset
        {
            AllowRepeats = false,
            BassNoteRange = new List<int> { 60 },
            TrebleNoteRange = new List<int> { 61 },
            CleffMode = CleffMode.Treble,
            Direction = Direction.Up,
            ForceRetry = false,
            Keys = new List<Key> { new(Letter.D, Accidental.Flat), new(Letter.C, Accidental.Natural) }
        };

        var session = new KeysSession(preset);

        var key = session.Key;

        for (int i = 0; i < 7; i++)
        {
            var note = NoteHelper.GetMidi(session.GetNotes().First());

            session.NotePlayed(note, 100);
        }

        Assert.NotEqual(key, session.Key);
    }

    [Fact]
    public void TestCleffs()
    {
        var preset = new KeysSessionPreset
        {
            AllowRepeats = false,
            BassNoteRange = new List<int> { 60 },
            TrebleNoteRange = new List<int> { 61 },
            CleffMode = CleffMode.Treble,
            Direction = Direction.Up,
            ForceRetry = false,
            Keys = new List<Key> { new(Letter.D, Accidental.Flat) }
        };

        var session = new KeysSession(preset);

        Assert.True(session.UseTrebleCleff);

        preset.CleffMode = CleffMode.Bass;

        session = new KeysSession(preset);

        Assert.False(session.UseTrebleCleff);
    }

    [Fact]
    public void TestForceRetry()
    {
        var preset = new KeysSessionPreset
        {
            AllowRepeats = false,
            BassNoteRange = new List<int> { 60 },
            TrebleNoteRange = new List<int> { 61 },
            CleffMode = CleffMode.Treble,
            Direction = Direction.Up,
            ForceRetry = false,
            Keys = new List<Key> { new(Letter.D, Accidental.Flat) }
        };

        var session = new KeysSession(preset);

        var first = NoteHelper.GetMidi(session.GetNotes().First());

        session.NotePlayed(first, 100);

        var second = session.GetNotes().First();

        session.NotePlayed(int.MaxValue, 100);

        Assert.Equal(second, session.GetNotes().First());

        preset.ForceRetry = true;

        session = new KeysSession(preset);

        var firstNote = session.GetNotes().First();

        session.NotePlayed(NoteHelper.GetMidi(firstNote), 100);

        session.NotePlayed(int.MaxValue, 100);

        Assert.Equal(firstNote, session.GetNotes().First());
    }

    [Fact]
    public void TestStartingNoteRange()
    {
        var preset = new KeysSessionPreset
        {
            AllowRepeats = false,
            BassNoteRange = new List<int> { 48 },
            TrebleNoteRange = new List<int> { 72 },
            CleffMode = CleffMode.Treble,
            Direction = Direction.Up,
            ForceRetry = false,
            Keys = new List<Key> { new(Letter.C, Accidental.Natural) }
        };

        var session = new KeysSession(preset);

        var first = session.GetNotes().First();

        Assert.Equal(first, new(Letter.C, 5, Accidental.Natural));

        preset.CleffMode = CleffMode.Bass;

        session = new KeysSession(preset);

        first = session.GetNotes().First();

        Assert.Equal(first, new(Letter.C, 3, Accidental.Natural));
    }

    [Fact]
    public void TestNotePlayed()
    {
        var preset = new KeysSessionPreset
        {
            AllowRepeats = false,
            BassNoteRange = new List<int> { 60 },
            TrebleNoteRange = new List<int> { 61 },
            CleffMode = CleffMode.Treble,
            Direction = Direction.Up,
            ForceRetry = false,
            Keys = new List<Key> { new(Letter.D, Accidental.Flat) }
        };

        var session = new KeysSession(preset);

        Assert.False(session.NotePlayed(int.MinValue, 0));
        Assert.False(session.NotePlayed(int.MinValue, 0));

        for (int i = 0; i < 7; i++)
        {
            var note = NoteHelper.GetMidi(session.GetNotes().First());

            Assert.True(session.NotePlayed(note, 100));
        }
    }

    [Fact]
    public void TestStatistics()
    {
        var preset = new KeysSessionPreset
        {
            AllowRepeats = false,
            BassNoteRange = new List<int> { 60 },
            TrebleNoteRange = new List<int> { 61 },
            CleffMode = CleffMode.Treble,
            Direction = Direction.Up,
            ForceRetry = false,
            Keys = new List<Key> { new(Letter.D, Accidental.Flat) }
        };

        var session = new KeysSession(preset);

        var first = session.Key;

        session.NotePlayed(int.MinValue, 0);
        session.NotePlayed(int.MinValue, 0);

        for (int i = 0; i < 7; i++)
        {
            var note = NoteHelper.GetMidi(session.GetNotes().First());

            session.NotePlayed(note, 100);
        }

        var second = session.Key;

        for (int i = 0; i < 7; i++)
        {
            var note = NoteHelper.GetMidi(session.GetNotes().First());

            session.NotePlayed(note, 100);
        }

        Assert.Equal(2, session.TotalScalesPlayed);
        Assert.Equal(14, session.TotalNotesPlayed);
        Assert.Equal(16, session.TotalAttempts);

        var results = session.GetKeyStatistics().ToDictionary(a => a.Key, b => b.Value);

        Assert.Equal(4, results[new Key(Letter.D, Accidental.Flat)].Attempts);
        Assert.Equal(2, results[new Key(Letter.D, Accidental.Flat)].TimesPlayed);
        Assert.Equal(1400, results[new Key(Letter.D, Accidental.Flat)].Latency);

        var stats = new Statistics();
        session.ApplyStatistics(stats);

        Assert.Equal(14, stats.TotalNotesPlayed);
        Assert.Equal(16, stats.TotalNotesAttempted);

        stats.LoadData();

        Assert.Single(stats.Keys);

        var key = stats.GetKey(new(Letter.D, Accidental.Flat));
        Assert.Equal(4, key.TotalAttempts);
        Assert.Equal(2, key.TotalTimesPlayed);
        Assert.Equal(1400, key.TotalLatency);
    }

    [Fact]
    public void TestValidation()
    {
        var preset = new KeysSessionPreset
        {
            AllowRepeats = false,
            BassNoteRange = new List<int> { 60 },
            TrebleNoteRange = new List<int> { 61 },
            CleffMode = CleffMode.Treble,
            Direction = Direction.Up,
            ForceRetry = false,
            Keys = new List<Key> { }
        };

        Assert.Throws<ArgumentException>(() => new KeysSession(preset));

        preset = new KeysSessionPreset
        {
            AllowRepeats = false,
            BassNoteRange = new List<int> { },
            TrebleNoteRange = new List<int> { 61 },
            CleffMode = CleffMode.Treble,
            Direction = Direction.Up,
            ForceRetry = false,
            Keys = new List<Key> { new(Letter.D, Accidental.Flat) }
        };

        Assert.Throws<ArgumentException>(() => new KeysSession(preset));

        preset = new KeysSessionPreset
        {
            AllowRepeats = false,
            BassNoteRange = new List<int> { 60 },
            TrebleNoteRange = new List<int> { },
            CleffMode = CleffMode.Treble,
            Direction = Direction.Up,
            ForceRetry = false,
            Keys = new List<Key> { new(Letter.D, Accidental.Flat) }
        };

        Assert.Throws<ArgumentException>(() => new KeysSession(preset));
    }
}
