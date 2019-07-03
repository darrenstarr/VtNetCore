function getTestInformation() {
    return {
        "name": "CUP—Cursor Position",
        "features": "CUP",
        "links": [
            "https://vt100.net/docs/vt510-rm/CUP.html"
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

    terminal.push(curses.CUP(10, 5).getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 9, 4);

    terminal.push(curses.CUP(8).getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 7, 0);

    terminal.push(curses.CUP().getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 0, 0);

    log.status("Passed?: " + ok);

    return ok;
}
