function draw(elementID, noteNames, noteDuration, timeSignatureTop, timeSignatureBottom, timeSignatureString, keySignatureString, clef) {

    try {
        clearBox(elementID);
    }
    catch {
        return;
    }

    const {
        Renderer,
        Stave,
        StaveNote,
        Voice,
        Formatter
    } = Vex.Flow;

    const div = document.getElementById(elementID);
    const renderer = new Renderer(div, Renderer.Backends.SVG);

    renderer.resize(500, 300);
    const context = renderer.getContext();

    const stave = new Stave(25, 90, 450);

    stave.addClef(clef).addTimeSignature(timeSignatureString);

    new Vex.KeySignature(keySignatureString).addToStave(stave);
    
    stave.setContext(context).draw();
    document.getElementById(elementID).firstChild.style = "width: 100%; height: 100%"

    if (noteNames.length > 0) {
        const staveNote = new StaveNote({
            keys: noteNames,
            duration: noteDuration,
            clef: clef
        });

        const notes = [staveNote];

        const voice = new Voice({
            num_beats: timeSignatureTop,
            beat_value: timeSignatureBottom
        });
        voice.addTickables(notes);

        new Formatter().joinVoices([voice]).format([voice], 300);
        voice.draw(context, stave);
    }
    else {
        try {
            new Formatter().format([], 300);
        }
        catch {
        }
    }
}

function clearBox(elementID) {
    document.getElementById(elementID).innerHTML = "";
}