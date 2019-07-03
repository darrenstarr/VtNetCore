function getTestInformation() {
    return {
        "name": "CUF—Cursor Forward",
        "features": "CUF",
        "links": [
            "https://vt100.net/docs/vt510-rm/CUF.html"
        ],
        "authors": [
            "Darren Starr <submux@hotmail.com>"
        ],
        "notes": ""
    };
}

function executeTest() {
    log.info("Test: CUF—Cursor Forward");

    let curses = new Curses();

    var terminal = host.newObj(Terminal);
    terminal.resizeView(80, 25);
    terminal.push(curses.CUD().CUD(3).CUD(0).getData());

    let ok = true;

    terminal.push(curses.CUF().getData());
    log.debug("(r=" + terminal.row + ", c=" + terminal.column + ")");
    if (!(terminal.row === 5 && terminal.column === 1))
        ok = false;
    log.debug("Ok(1)? " + (terminal.row === 5 && terminal.column === 1));

    terminal.push(curses.CUF(3).getData());
    log.debug("(r=" + terminal.row + ", c=" + terminal.column + ")");
    if (!(terminal.row === 5 && terminal.column === 4))
        ok = false;
    log.debug("Ok(2)? " + (terminal.row === 5 && terminal.column === 4));

    terminal.push(curses.CUF(0).getData());
    log.debug("(r=" + terminal.row + ", c=" + terminal.column + ")");
    if (!(terminal.row === 5 && terminal.column === 5))
        ok = false;
    log.debug("Ok(3)? " + (terminal.row === 5 && terminal.column === 5));

    log.status("Passed?: " + ok);

    return ok;
}
