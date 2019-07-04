function getTestInformation() {
    return {
        "name": "DA1—Primary Device Attributes",
        "features": "DA1",
        "links": [
            "https://vt100.net/docs/vt510-rm/DA1.html"
        ],
        "authors": [
            "Darren Starr <submux@hotmail.com>"
        ],
        "notes": ""
    };
}

function executeTest() {
    log.info("Test: " + getTestInformation().name);

    let curses = new Curses();

    var terminal = host.newObj(Terminal);
    terminal.resizeView(80, 25);

    let ok = true;

    var seq = curses.DA1().getData();
    terminal.push(seq);
    var b = terminal.getSendBuffer();

    var re = /<esc>\[\?64;(\d+;{1,3})*\d{1,3}c/;
    var m = re.exec(b);

    if (m.length > 0)
        log.debug("Resulting buffer is valid: " + b);
    else {
        log.debug("Resulting buffer is invalid: " + b);
        ok = false;
    }

    log.status("Passed?: " + ok);

    return ok;
}
