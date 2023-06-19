using BlueNotation.Music;

namespace BlueNotationTests.Music;

public class NoteHelperTest
{
    [Fact]
    public void TestGetNote()
    {
        Assert.Equal(new Note(Letter.C, 4, Accidental.Natural), NoteHelper.GetNote(60));
        Assert.Equal(new Note(Letter.C, 4, Accidental.Sharp), NoteHelper.GetNote(61));
        Assert.Equal(new Note(Letter.D, 4, Accidental.Natural), NoteHelper.GetNote(62));
        Assert.Equal(new Note(Letter.E, 4, Accidental.Flat), NoteHelper.GetNote(63));
        Assert.Equal(new Note(Letter.E, 4, Accidental.Natural), NoteHelper.GetNote(64));
        Assert.Equal(new Note(Letter.F, 4, Accidental.Natural), NoteHelper.GetNote(65));
        Assert.Equal(new Note(Letter.F, 4, Accidental.Sharp), NoteHelper.GetNote(66));
        Assert.Equal(new Note(Letter.G, 4, Accidental.Natural), NoteHelper.GetNote(67));
        Assert.Equal(new Note(Letter.A, 4, Accidental.Flat), NoteHelper.GetNote(68));
        Assert.Equal(new Note(Letter.A, 4, Accidental.Natural), NoteHelper.GetNote(69));
        Assert.Equal(new Note(Letter.B, 4, Accidental.Flat), NoteHelper.GetNote(70));
        Assert.Equal(new Note(Letter.B, 4, Accidental.Natural), NoteHelper.GetNote(71));
        Assert.Equal(new Note(Letter.C, 5, Accidental.Natural), NoteHelper.GetNote(72));
        Assert.Equal(new Note(Letter.C, 5, Accidental.Sharp), NoteHelper.GetNote(73));


        Assert.Equal(new Note(Letter.C, -1, Accidental.Natural), NoteHelper.GetNote(0));
        Assert.Equal(new Note(Letter.C, -2, Accidental.Natural), NoteHelper.GetNote(-12));
    }

    [Fact]
    public void TestGetNoteWithKey()
    {
        var cMajor = new Key(Letter.C, Accidental.Natural);

        Assert.Equal(new Note(Letter.C, 4, Accidental.Natural), NoteHelper.GetNote(60, cMajor));
        Assert.Equal(new Note(Letter.C, 4, Accidental.Sharp), NoteHelper.GetNote(61, cMajor));
        Assert.Equal(new Note(Letter.B, 4, Accidental.Natural), NoteHelper.GetNote(71, cMajor));
        Assert.Equal(new Note(Letter.E, 4, Accidental.Flat), NoteHelper.GetNote(63, cMajor));

        var cFlatMajor = new Key(Letter.C, Accidental.Flat);

        Assert.Equal(new Note(Letter.C, 5, Accidental.Flat), NoteHelper.GetNote(71, cFlatMajor));
        Assert.Equal(new Note(Letter.C, 5, Accidental.Natural), NoteHelper.GetNote(72, cFlatMajor));

        var cSharpMajor = new Key(Letter.C, Accidental.Sharp);

        Assert.Equal(new Note(Letter.C, 4, Accidental.Sharp), NoteHelper.GetNote(61, cSharpMajor));
        Assert.Equal(new Note(Letter.B, 3, Accidental.Sharp), NoteHelper.GetNote(60, cSharpMajor));
        Assert.Equal(new Note(Letter.E, 4, Accidental.Sharp), NoteHelper.GetNote(65, cSharpMajor));
        Assert.Equal(new Note(Letter.D, 4, Accidental.Natural), NoteHelper.GetNote(62, cSharpMajor));

        var dMajor = new Key(Letter.D, Accidental.Natural);

        Assert.Equal(new Note(Letter.C, 4, Accidental.Sharp), NoteHelper.GetNote(61, dMajor));
        Assert.Equal(new Note(Letter.C, 4, Accidental.Natural), NoteHelper.GetNote(60, dMajor));
        Assert.Equal(new Note(Letter.F, 4, Accidental.Natural), NoteHelper.GetNote(65, dMajor));
        Assert.Equal(new Note(Letter.E, 4, Accidental.Flat), NoteHelper.GetNote(63, dMajor));

        Assert.Throws<ArgumentException>(() => NoteHelper.GetNote(60, new Key(Letter.D, Accidental.Sharp)));
        Assert.Throws<ArgumentException>(() => NoteHelper.GetNote(60, new Key(Letter.A, Accidental.Sharp)));
        Assert.Throws<ArgumentException>(() => NoteHelper.GetNote(60, new Key(Letter.F, Accidental.Flat)));
    }

    [Fact]
    public void TestGetMidi()
    {
        Assert.Equal(60, NoteHelper.GetMidi(new(Letter.C, 4, Accidental.Natural)));
        Assert.Equal(0, NoteHelper.GetMidi(new(Letter.C, -1, Accidental.Natural)));
        Assert.Equal(102, NoteHelper.GetMidi(new(Letter.F, 7, Accidental.Sharp)));
        Assert.Equal(40, NoteHelper.GetMidi(new(Letter.E, 2, Accidental.Natural)));
        Assert.Equal(60, NoteHelper.GetMidi(new(Letter.B, 3, Accidental.Sharp)));
        Assert.Equal(59, NoteHelper.GetMidi(new(Letter.C, 4, Accidental.Flat)));
    }

    [Fact]
    public void TestGetScale()
    {
        var notes = NoteHelper.GetScale(60, new(Letter.C, Accidental.Natural)).ToArray();

        Assert.Equal(7, notes.Length);
        Assert.Equal(60, notes[0]);
        Assert.Equal(62, notes[1]);
        Assert.Equal(64, notes[2]);
        Assert.Equal(65, notes[3]);
        Assert.Equal(67, notes[4]);
        Assert.Equal(69, notes[5]);
        Assert.Equal(71, notes[6]);

        notes = NoteHelper.GetScale(60, new(Letter.G, Accidental.Natural)).ToArray();

        Assert.Equal(7, notes.Length);
        Assert.Equal(60, notes[0]);
        Assert.Equal(62, notes[1]);
        Assert.Equal(64, notes[2]);
        Assert.Equal(66, notes[3]);
        Assert.Equal(67, notes[4]);
        Assert.Equal(69, notes[5]);
        Assert.Equal(71, notes[6]);

        notes = NoteHelper.GetScale(45, new(Letter.D, Accidental.Flat)).ToArray();

        Assert.Equal(7, notes.Length);
        Assert.Equal(46, notes[0]);
        Assert.Equal(48, notes[1]);
        Assert.Equal(49, notes[2]);
        Assert.Equal(51, notes[3]);
        Assert.Equal(53, notes[4]);
        Assert.Equal(54, notes[5]);
        Assert.Equal(56, notes[6]);
    }
}