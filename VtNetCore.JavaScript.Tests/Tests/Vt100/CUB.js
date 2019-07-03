function getTestInformation() {
    return {
        "name": "CUB—Cursor Backward",
        "features": "CUB",
        "links": [
            "https://vt100.net/docs/vt510-rm/CUB.html"
        ],
        "authors": [
            "Darren Starr <submux@hotmail.com>"
        ],
        "standards": [
            "VT100"
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

    terminal.push(curses.CUD().CUD(3).CUD(0).CUF().CUF(3).CUF(0).CUU().CUU(3).CUU(0).getData());

    terminal.push(curses.CUB().getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 0, 4);

    terminal.push(curses.CUB(3).getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 0, 1);

    terminal.push(curses.CUB(0).getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 0, 0);

    log.status("Passed?: " + ok);

    return ok;
}
