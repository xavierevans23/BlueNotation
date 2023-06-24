function saveLocalStorage(key, text) {
    localStorage.setItem(key, text);
}

function loadLocalStorage(key) {
    return localStorage.getItem(key);
}