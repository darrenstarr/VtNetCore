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

    static dumpSequence(seq) {
        let str = "";
        for (var i = 0; i < seq.length; i++) {
            switch (seq[i]) {
                case 0: str += "<nil>"; break;
                case 1: str += "<soh>"; break;
                case 2: str += "<stx>"; break;
                case 3: str += "<etx>"; break;
                case 4: str += "<eot>"; break;
                case 5: str += "<enq>"; break;
                case 6: str += "<ack>"; break;
                case 7: str += "<bel>"; break;
                case 8: str += "<bs>"; break;
                case 9: str += "<ht>"; break;
                case 10: str += "<lf>"; break;
                case 11: str += "<vt>"; break;
                case 12: str += "<ff>"; break;
                case 13: str += "<cr>"; break;
                case 14: str += "<si>"; break;
                case 15: str += "<so>"; break;
                case 16: str += "<dle>"; break;
                case 17: str += "<dc1>"; break;
                case 18: str += "<dc2>"; break;
                case 19: str += "<dc3>"; break;
                case 20: str += "<dc4>"; break;
                case 21: str += "<nak>"; break;
                case 22: str += "<syn>"; break;
                case 23: str += "<etb>"; break;
                case 24: str += "<can>"; break;
                case 25: str += "<em>"; break;
                case 26: str += "<sub>"; break;
                case 27: str += "<esc>"; break;
                case 28: str += "<fs>"; break;
                case 29: str += "<gs>"; break;
                case 30: str += "<rs>"; break;
                case 31: str += "<us>"; break;
                case 127: str += "<del>"; break;
                default:
                    if (seq[i] < 128) {
                        str += String.fromCharCode(seq[i]);
                    } else {
                        str += "{" + seq[i].toString(16) + "}";
                    }
                    break;
            }
        }
        log.debug(str);
    }
}