function draw(elementID, noteNames, noteDuration, timeSignatureTop, timeSignatureBottom, timeSignatureString, keySignatureString, clef) {

    clearBox(elementID);
    const {
        Renderer,
        Stave,
        StaveNote,
        Voice,
        Formatter
    } = Vex.Flow;

    const div = document.getElementById(elementID);
    const renderer = new Renderer(div, Renderer.Backends.SVG);

    renderer.resize(500, 200);
    const context = renderer.getContext();

    const stave = new Stave(40, 40, 400);

    stave.addClef(clef).addTimeSignature(timeSignatureString);

    new Vex.KeySignature(keySignatureString).addToStave(stave);
    
    stave.setContext(context).draw();
    document.getElementById(elementID).firstChild.style = "width: 100%; height: 100%"

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
    
    new Formatter().joinVoices([voice]).format([voice], 350);
    
    voice.draw(context, stave);    
}

function clearBox(elementID) {
    document.getElementById(elementID).innerHTML = "";
}