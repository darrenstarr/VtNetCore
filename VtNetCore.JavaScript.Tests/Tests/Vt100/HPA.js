function getTestInformation() {
    return {
        "name": "HPA—Horizontal Position Absolute",
        "features": "HPA",
        "links": [
            "https://vt100.net/docs/vt510-rm/HPA.html"
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

    terminal.push(curses.HPA(5).getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 0, 4);

    terminal.push(curses.CUP(6,7).HPA(5).getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 5, 4);

    terminal.push(curses.HPA().getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 5, 0);

    log.status("Passed?: " + ok);

    return ok;
}
