function getTestInformation() {
    return {
        "name": "CUU—Cursor Up",
        "features": "CUU",
        "links": [
            "https://vt100.net/docs/vt510-rm/CUU.html"
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
    terminal.push(curses.CUD().CUD(3).CUD(0).CUF().CUF(3).CUF(0).getData());

    let ok = true;

    terminal.push(curses.CUU().getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 4, 5);

    terminal.push(curses.CUU(3).getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 1, 5);

    terminal.push(curses.CUU(0).getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 0, 5);

    terminal.push(curses.CUU().getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 0, 5);
   
    log.status("Passed?: " + ok);

    return ok;
}
