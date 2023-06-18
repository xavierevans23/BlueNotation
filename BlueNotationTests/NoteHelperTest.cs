using BlueNotation.Music;

namespace BlueNotationTests;

public class NoteHelperTest
{
    [Fact]
    public void TestGetNote()
    {
        Assert.Equal(new Note(Letter.C, 4, Accidental.Natural),NoteHelper.GetNote(60));
        Assert.Equal(new Note(Letter.C, 4, Accidental.Sharp),NoteHelper.GetNote(61));
        Assert.Equal(new Note(Letter.D, 4, Accidental.Natural),NoteHelper.GetNote(62));
        Assert.Equal(new Note(Letter.E, 4, Accidental.Flat),NoteHelper.GetNote(63));
        Assert.Equal(new Note(Letter.E, 4, Accidental.Natural),NoteHelper.GetNote(64));
        Assert.Equal(new Note(Letter.F, 4, Accidental.Natural),NoteHelper.GetNote(65));
        Assert.Equal(new Note(Letter.F, 4, Accidental.Sharp),NoteHelper.GetNote(66));
        Assert.Equal(new Note(Letter.G, 4, Accidental.Natural),NoteHelper.GetNote(67));
        Assert.Equal(new Note(Letter.A, 4, Accidental.Flat),NoteHelper.GetNote(68));
        Assert.Equal(new Note(Letter.A, 4, Accidental.Natural),NoteHelper.GetNote(69));
        Assert.Equal(new Note(Letter.B, 4, Accidental.Flat),NoteHelper.GetNote(70));
        Assert.Equal(new Note(Letter.B, 4, Accidental.Natural),NoteHelper.GetNote(71));
        Assert.Equal(new Note(Letter.C, 5, Accidental.Natural),NoteHelper.GetNote(72));
        Assert.Equal(new Note(Letter.C, 5, Accidental.Sharp),NoteHelper.GetNote(73));
        
        
        Assert.Equal(new Note(Letter.C, -1, Accidental.Natural),NoteHelper.GetNote(0));
        Assert.Equal(new Note(Letter.C, -2, Accidental.Natural),NoteHelper.GetNote(-12));
    }

    [Fact]
    public void TestGetNoteWithKey()
    {

    }

    [Fact]
    public void TestGetMidi()
    {

    }

    [Fact]
    public void TestGetScale()
    {

    }
}