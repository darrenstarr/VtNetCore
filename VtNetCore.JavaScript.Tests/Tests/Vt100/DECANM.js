function getTestInformation() {
    return {
        "name": "DECANM—ANSI Mode",
        "features": "Vt52",
        "links": [
            "https://vt100.net/docs/vt510-rm/DECANM.html"
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
    terminal.push(curses.CUD().CUD(3).CUD(0).CUF().CUF(3).CUF(0).DECANM().getData());

    let ok = true;

    terminal.push(curses.Vt52CUU().getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 4, 5);

    terminal.push(curses.Vt52CUD().getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 5, 5);

    terminal.push(curses.Vt52CUF().getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 5, 6);

    terminal.push(curses.Vt52CUB().getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 5, 5);

    terminal.push(curses.Vt52Home().getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 0, 0);

    terminal.push(curses.Vt52DirectCursorAddress(5,5).getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 4, 4);

    terminal.push(curses.Vt52EnterAnsiMode().getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 4, 4);

    terminal.push(curses.CUD().getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 5, 4);

    terminal.resizeView(5, 5);
    terminal.push(curses.DECALN().CUP(3, 3).DECANM().getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 2, 2);

    terminal.push(curses.Vt52EraseToEndOfLine().getData());
    ok = TerminalHelpers.checkScreenText(terminal, ok, "EEEEE\nEEEEE\nEE   \nEEEEE\nEEEEE");

    terminal.push(curses.Vt52EraseToEndOfScreen().getData());
    ok = TerminalHelpers.checkScreenText(terminal, ok, "EEEEE\nEEEEE\nEE   \n     \n     ");

    terminal.push(curses.Vt52ReverseLineFeed().getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 1, 2);

    terminal.push(curses.Vt52EnterAnsiMode().getData());

    log.status("Passed?: " + ok);

    return ok;
}
