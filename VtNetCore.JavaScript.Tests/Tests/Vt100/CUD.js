function getTestInformation() {
    return {
        "name": "CUD—Cursor Down",
        "features": "CUD",
        "links": [
            "https://vt100.net/docs/vt510-rm/CUD.html"
        ],
        "authors": [
            "Darren Starr <submux@hotmail.com>"
        ],
        "notes": ""
    };
}

function executeTest() {
    log.info("Test: CUD—Cursor Down");

    let curses = new Curses();

    var terminal = host.newObj(Terminal);
    terminal.resizeView(80, 25);    

    let ok = true;

    terminal.push(curses.CUD().getData());
    log.debug("(r=" + terminal.row + ", c=" + terminal.column + ")");
    if (!(terminal.row === 1 && terminal.column === 0))
        ok = false;
    log.debug("Ok(1)? " + (terminal.row === 1 && terminal.column === 0));

    terminal.push(curses.CUD(3).getData());
    log.debug("(r=" + terminal.row + ", c=" + terminal.column + ")");
    if (!(terminal.row === 4 && terminal.column === 0))
        ok = false;
    log.debug("Ok(2)? " + (terminal.row === 4 && terminal.column === 0));

    terminal.push(curses.CUD(0).getData());
    log.debug("(r=" + terminal.row + ", c=" + terminal.column + ")");
    if (!(terminal.row === 5 && terminal.column === 0))
        ok = false;
    log.debug("Ok(3)? " + (terminal.row === 5 && terminal.column === 0));

    log.status("Passed?: " + ok);

    return ok;
}
