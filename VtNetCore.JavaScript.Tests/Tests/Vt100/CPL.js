function getTestInformation() {
    return {
        "name": "CPL—Cursor Previous Line",
        "features": "CPL",
        "links": [
            "https://vt100.net/docs/vt510-rm/CPL.html"
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

    terminal.push(curses.Text("   ").CNL().Text("   ").CNL(2).CNL().getData());

    terminal.push(curses.Text("   ").getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 4, 3);

    terminal.push(curses.CPL().getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 3, 0);

    terminal.push(curses.Text("   ").getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 3, 3);

    terminal.push(curses.CPL(2).getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 1, 0);

    terminal.push(curses.CPL(0).getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 0, 0);

    log.status("Passed?: " + ok);

    return ok;
}
