function getTestInformation() {
    return {
        "name": "CHA—Cursor Horizontal Absolute",
        "features": "CHA",
        "links": [
            "https://vt100.net/docs/vt510-rm/CHA.html"
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

    terminal.push(curses.LF().getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 1, 0);

    terminal.push(curses.CHA(20).getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 1, 19);

    terminal.push(curses.CHA().getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 1, 0);

    log.status("Passed?: " + ok);

    return ok;
}
