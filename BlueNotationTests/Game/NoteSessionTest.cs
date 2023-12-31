﻿using BlueNotation.Data;
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
            BassNoteRange = new() { 60, 62 },
            ClefMode = ClefMode.Treble,
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
            TrebleNoteRange = new() { 60, 62 },
            BassNoteRange = new() { 60, 62 },
            ClefMode = ClefMode.Bass,
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
            BassNoteRange = new() { 60, 62 },
            ClefMode = ClefMode.Treble,
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
            BassNoteRange = new() { 60, 62 },
            ClefMode = ClefMode.Treble,
            AllowRepeats = false,
            MaxNotes = 5,
            MinNotes = 5
        };

        var session = new NotesSession(preset);
        Assert.Equal(2, session.GetNotes().Count());

        preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { 60, 62, 64, 65, 67 },
            ClefMode = ClefMode.Treble,
            BassNoteRange = new() { 60, 62 },
            AllowRepeats = false,
            MaxNotes = 5,
            MinNotes = 5
        };

        session = new NotesSession(preset);
        Assert.Equal(5, session.GetNotes().Count());
    }

    [Fact]
    public void TestNoteClef()
    {
        var preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { 60, 62 },
            BassNoteRange = new() { 60, 62 },
            ClefMode = ClefMode.Treble,
            AllowRepeats = false,
            MaxNotes = 5,
            MinNotes = 5
        };

        var session = new NotesSession(preset);
        Assert.True(session.UseTrebleClef);

        preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { 60, 62 },
            BassNoteRange = new() { 60, 62 },
            ClefMode = ClefMode.Bass,
            AllowRepeats = false,
            MaxNotes = 5,
            MinNotes = 5
        };

        session = new NotesSession(preset);
        Assert.False(session.UseTrebleClef);
    }

    [Fact]
    public void TestNotePlayed()
    {
        var preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { 60, 62 },
            BassNoteRange = new() { 60, 62 },
            ClefMode = ClefMode.Treble,
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
        Assert.Equal(new Key(Letter.C, Accidental.Natural), new NotesSession(new() { TrebleNoteRange = new() { 60, 62 }, BassNoteRange = new() { 60, 62 }, }).Key);
    }

    [Fact]
    public void TestStatistics()
    {
        var preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { 60, 62 },
            BassNoteRange = new() { 60, 62 },
            ClefMode = ClefMode.Treble,
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

        Assert.Equal(2, session.TotalPerfect);

        var history = session.GetTrebleNoteStatistics().ToDictionary(a => a.Key, b => b.Value);

        Assert.True(history.ContainsKey(note));
        Assert.Equal(2, history.Count);

        var stats = new Statistics() { TotalNotesAttempted = 2, TotalNotesPlayed = 1 };

        session.ApplyStatistics(stats);

        var firstMidi = NoteHelper.GetMidi(note);
        Assert.Equal(6, stats.TotalNotesAttempted);
        Assert.Equal(4, stats.TotalNotesPlayed);

        Assert.Equal(2, stats.GetTrebleNote(firstMidi).TotalAttempts);
        Assert.Equal(2, stats.GetTrebleNote(firstMidi).TotalTimesPlayed);
        Assert.Equal(150, stats.GetTrebleNote(firstMidi).TotalLatency);

        firstMidi = firstMidi == 60 ? 62 : 60;

        Assert.Equal(2, stats.GetTrebleNote(firstMidi).TotalAttempts);
        Assert.Equal(1, stats.GetTrebleNote(firstMidi).TotalTimesPlayed);
        Assert.Equal(200, stats.GetTrebleNote(firstMidi).TotalLatency);
    }

    [Fact]
    public void TestValidation()
    {
        var preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { 60, 62 },
            ClefMode = ClefMode.Treble,
            AllowRepeats = false,
            MaxNotes = 1,
            MinNotes = 0
        };

        Assert.Throws<ArgumentException>(() => new NotesSession(preset));

        preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { 60, 62 },
            ClefMode = ClefMode.Treble,
            AllowRepeats = false,
            MaxNotes = 0,
            MinNotes = 1
        };

        Assert.Throws<ArgumentException>(() => new NotesSession(preset));

        preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { 60, 62 },
            ClefMode = ClefMode.Treble,
            AllowRepeats = false,
            MaxNotes = 5,
            MinNotes = 6
        };

        Assert.Throws<ArgumentException>(() => new NotesSession(preset));

        preset = new NotesSessionPreset
        {
            TrebleNoteRange = new() { },
            ClefMode = ClefMode.Treble,
            AllowRepeats = false,
            MaxNotes = 1,
            MinNotes = 1
        };

        Assert.Throws<ArgumentException>(() => new NotesSession(preset));

        preset = new NotesSessionPreset
        {
            BassNoteRange = new() { },
            ClefMode = ClefMode.Treble,
            AllowRepeats = false,
            MaxNotes = 1,
            MinNotes = 1
        };

        Assert.Throws<ArgumentException>(() => new NotesSession(preset));
    }
}
