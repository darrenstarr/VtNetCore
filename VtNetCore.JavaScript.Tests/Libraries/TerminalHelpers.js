class TerminalHelpers {
    static checkPosition(terminal, ok, row, column) {
        if (terminal.row === row && terminal.column === column)
            log.debug("Position is (r=" + terminal.row + ", c=" + terminal.column + ")... ok");
        else {
            log.debug("Position is (r=" + terminal.row + ", c=" + terminal.column + ")... expected (r=" + row + ", c=" + column + ")");
            ok = false;
        }

        return ok;
    }

    static checkScreenText(terminal, ok, expectedText) {
        let text = terminal.screenText;

        if (expectedText === text)
            log.debug("Screen text is as expected!");
        else {
            log.debug("Expected");
            log.dump(expectedText);
            log.debug("Received");
            log.dump(text);
            ok = false;
        }

        return ok;
    }
}