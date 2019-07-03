function getTestInformation() {
    return {
        "name": "DECALN—Screen Alignment Pattern",
        "features": "DECALN",
        "links": [
            "https://vt100.net/docs/vt510-rm/DECALN.html"
        ],
        "authors": [
            "Darren Starr <submux@hotmail.com>"
        ],
        "notes": `
DECALN or screen alignment pattern fills the contents of the screen
with the capital letter 'E' in all spaces. In addition, it moves
the cursor position to "home" or 0,0.
`
    };
}

function executeTest() {
    log.info("Test: DECALN—Screen Alignment Pattern");

    let expectedText = "EEEEE\nEEEEE\nEEEEE\nEEEEE\nEEEEE";

    let data = new Uint8Array([
        0x1B,
        '#'.charCodeAt(0),
        '8'.charCodeAt(0)
    ]);

    var terminal = host.newObj(Terminal);
    terminal.resizeView(5, 5);
    terminal.push(data);
    log.dump(terminal.screenText);
    log.debug("Text matches?: " + (expectedText === terminal.screenText));
    log.debug("Column = 0?: " + (terminal.column === 0));
    log.debug("Row = 0?: " + (terminal.row === 0));

    let ok =
        expectedText === terminal.screenText &&
        terminal.column === 0 &&
        terminal.row === 0;

    log.status("Passed?: " + ok);

    return ok;
}
