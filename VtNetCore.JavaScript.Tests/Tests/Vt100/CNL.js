function getTestInformation() {
    return {
        "name": "CNL—Cursor Next Line",
        "features": "CNL",
        "links": [
            "https://vt100.net/docs/vt510-rm/CNL.html"
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

    terminal.push(curses.Text("   ").getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 0, 3);

    terminal.push(curses.CNL().getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 1, 0);

    terminal.push(curses.Text("   ").getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 1, 3);

    terminal.push(curses.CNL(2).getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 3, 0);

    terminal.push(curses.CNL(0).getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 4, 0);

    log.status("Passed?: " + ok);

    return ok;
}
