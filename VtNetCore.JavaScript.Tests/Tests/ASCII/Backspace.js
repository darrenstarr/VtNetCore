function getTestInformation() {
    return {
        "name": "BS Backspace",
        "features": "BS",
        "links": [
            "http://unicode.org/L2/L2006/06388-review-incits4.pdf"
        ],
        "authors": [
            "Darren Starr <submux@hotmail.com>"
        ],
        "notes":
`ASCII defines backspace as: A format effector that causes the active position to move one character position backwards.
`
    };
}

function executeTest() {
    log.info("Test: BS Backspace");

    let curses = new Curses();

    var terminal = host.newObj(Terminal);
    terminal.resizeView(5, 5);    

    let ok = true;

    // Backspace should only move the cursor
    terminal.push(curses.Text("ABC\b").getData());
    log.debug("(r=" + terminal.row + ", c=" + terminal.column + ")");
    if (!(terminal.row === 0 && terminal.column === 2))
        ok = false;
    log.debug("Ok(1)? " + (terminal.row === 0 && terminal.column === 2));

    // It should not change the text
    let expectedText = "ABC  \n     \n     \n     \n     ";
    log.dump(terminal.screenText);
    if (!(terminal.screenText === expectedText))
        ok = false;
    log.debug("Ok(2) text match?: " + (terminal.screenText === expectedText));

    // Move to row 4, column 2
    terminal.push(curses.CUP(4, 2).getData());
    log.debug("(r=" + terminal.row + ", c=" + terminal.column + ")");
    if (!(terminal.row === 3 && terminal.column === 1))
        ok = false;
    log.debug("Ok(3) CUP? " + (terminal.row === 3 && terminal.column === 1));

    // Move backspace one, should be at the leftmost column now
    terminal.push(curses.Text('\b').getData());
    log.debug("(r=" + terminal.row + ", c=" + terminal.column + ")");
    if (!(terminal.row === 3 && terminal.column === 0))
        ok = false;
    log.debug("Ok(4) BS? " + (terminal.row === 3 && terminal.column === 0));

    // Move backspace again... this cursor should not have moved
    terminal.push(curses.Text('\b').getData());
    log.debug("(r=" + terminal.row + ", c=" + terminal.column + ")");
    if (!(terminal.row === 3 && terminal.column === 0))
        ok = false;
    log.debug("Ok(5) BS? " + (terminal.row === 3 && terminal.column === 0));

    log.status("Passed?: " + ok);

    return ok;
}
