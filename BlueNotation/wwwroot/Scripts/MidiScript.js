let midiStatus = 'ready';
let midiName = '';
let midiError = '';

function midiStart() {
    if (WebMidi.supported) {
        WebMidi.addListener('connected', debounce(updateDevice, 1000));
        WebMidi.addListener('disconnected', debounce(updateDevice, 1000));

        WebMidi
            .enable()
            .then(updateDevice)
            .catch(err => onMidiError(err));
    }
    else {
        midiStatus = 'unsupported';
        statusUpdate();
    }
}

function onMidiError(err) {
    midiError = err;
    midiStatus = 'error';
    statusUpdate();
}

function statusUpdate() {
    DotNet.invokeMethodAsync('BlueNotation', 'MidiEvent', midiStatus, midiName, midiError);
}

function updateDevice() {

    midiStatus = 'noDevices';

    if (WebMidi.inputs.length < 1) {
        statusUpdate();
        return;
    }

    midiStatus = 'connected';

    const device = WebMidi.inputs[0];
    device.removeListener();

    midiName = device.name;

    device.addListener("noteon", e => {
        notePressed(e.data[1]);
    });

    statusUpdate();
}

function notePressed(note) {
    DotNet.invokeMethodAsync('BlueNotation', 'NotePlayed', note);
}

function debounce(func, timeout = 300) {
    let timer;
    return (...args) => {
        clearTimeout(timer);
        timer = setTimeout(() => { func.apply(this, args); }, timeout);
    };
}