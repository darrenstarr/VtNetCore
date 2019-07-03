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
        "notes": ""
    };
}

function executeTest() {
    log.info("Test: CUU—Cursor Up");

    let curses = new Curses();

    var terminal = host.newObj(Terminal);
    terminal.resizeView(80, 25);
    terminal.push(curses.CUD().CUD(3).CUD(0).CUF().CUF(3).CUF(0).getData());

    let ok = true;

    terminal.push(curses.CUU().getData());
    log.debug("(r=" + terminal.row + ", c=" + terminal.column + ")");
    if (!(terminal.column === 5 && terminal.row === 4))
        ok = false;
    log.debug("Ok(1)? " + (terminal.column === 5 && terminal.row === 4));

    terminal.push(curses.CUU(3).getData());
    log.debug("(r=" + terminal.row + ", c=" + terminal.column + ")");
    if (!(terminal.column === 5 && terminal.row === 1))
        ok = false;
    log.debug("Ok(2)? " + (terminal.column === 5 && terminal.row === 1));

    terminal.push(curses.CUU(0).getData());
    if (!(terminal.column === 5 && terminal.row === 0))
        ok = false;
    log.debug("(r=" + terminal.row + ", c=" + terminal.column + ")");
    log.debug("Ok(3)? " + (terminal.column === 5 && terminal.row === 0));

    terminal.push(curses.CUU().getData());
    if (!(terminal.column === 5 && terminal.row === 0))
        ok = false;
    log.debug("(r=" + terminal.row + ", c=" + terminal.column + ")");
    log.debug("Ok(4)? " + (terminal.column === 5 && terminal.row === 0));
   
    log.status("Passed?: " + ok);

    return ok;
}
