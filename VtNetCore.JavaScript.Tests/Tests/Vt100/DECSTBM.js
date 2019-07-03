function getTestInformation() {
    return {
        "name": "DECSTBM—Set Top and Bottom Margins",
        "features": "DECSTBM",
        "links": [
            "https://vt100.net/docs/vt510-rm/DECBI.html"
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
    terminal.resizeView(5, 5);

    let ok = true;

    terminal.push(curses.Text("1111122222333334444455555").DECSTBM(2, 4).getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 0, 0);

    terminal.push(curses.CUP(4, 3).getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 3, 2);

    terminal.push(curses.Text("ABCDE").getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 3, 2);

    ok = TerminalHelpers.checkScreenText(terminal, ok, "11111\n33333\n44ABC\nDE   \n55555");

    terminal.push(curses.DECSTBM().getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 0, 0);

    terminal.push(curses.Text("ABCDE").CUP(4, 3).Text("ABCDE").getData());
    ok = TerminalHelpers.checkPosition(terminal, ok, 4, 2);
    ok = TerminalHelpers.checkScreenText(terminal, ok, "ABCDE\n33333\n44ABC\nDEABC\nDE555");

    log.status("Passed?: " + ok);

    return ok;
}
