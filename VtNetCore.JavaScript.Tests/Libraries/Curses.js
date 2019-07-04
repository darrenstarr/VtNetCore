class Curses {
    constructor() {
        this.data = new Uint8Array(0);
    }

    getData() {
        var x = this.data;
        this.data = new Uint8Array(0);
        return x;
    }

    pushByte(val) {
        let x = new Uint8Array(this.data.length + 1);
        x.set(this.data);
        x[this.data.length] = val;
        this.data = x;

        return this;
    }

    pushNum(value) {
        return this.Text(value.toString());
    }

    pushCsiCommand(p0, cmd) {
        this.CSI();
        if (p0 >= 0)
            this.pushNum(p0);

        this.pushByte(cmd.charCodeAt(0));

        return this;
    }

    Text(str) {
        let x = new Uint8Array(this.data.length + str.length);
        x.set(this.data);

        for (var i = 0; i < str.length; i++)
            x[this.data.length + i] = str.charCodeAt(i);

        this.data = x;

        return this;
    }

    DECBI() {
        return this.ESC().Text('6');
    }

    CHA(count = -1) {
        this.pushCsiCommand(count, 'G');

        return this;
    }

    CNL(count = -1) {
        this.pushCsiCommand(count, 'E');

        return this;
    }

    CPL(count = -1) {
        this.pushCsiCommand(count, 'F');

        return this;
    }

    CSI() {
        this.ESC();
        this.pushByte('['.charCodeAt(0));
        return this;
    }

    CUB(count = -1) {
        this.pushCsiCommand(count, 'D');

        return this;
    }

    CUD(count = -1) {
        this.pushCsiCommand(count, 'B');

        return this;
    }

    CUF(count = -1) {
        this.pushCsiCommand(count, 'C');

        return this;
    }

    CUU(count = -1) {
        this.pushCsiCommand(count, 'A');

        return this;
    }

    CUP(row = -1, column = -1) {
        this.CSI();
        if (row >= 0) {
            this.pushNum(row);

            if (column >= 0) {
                this.Text(';');
                this.pushNum(column);
            }
        }

        this.Text('H');

        return this;
    }

    DA1() {
        this.CSI().Text('c');

        return this;
    }

    DECALN() {
        this.ESC().Text("#8");
        return this;
    }

    DECANM() {
        this.CSI().Text("?2l");
        return this;
    }

    DECSTBM(top = -1, bottom = -1) {
        this.CSI();
        if (top >= 0) {
            this.pushNum(top);

            if (bottom >= 0) {
                this.Text(';');
                this.pushNum(bottom);
            }
        }

        this.Text('r');

        return this;
    }

    ESC() {
        this.pushByte(0x1b);
        return this;
    }

    HPA(column = -1) {
        this.pushCsiCommand(column, '`');

        return this;
    }

    LF() {
        this.Text('\n');
        return this;
    }

    Vt52CUU() {
        this.ESC().Text("A");
        return this;
    }

    Vt52CUD() {
        this.ESC().Text("B");
        return this;
    }

    Vt52CUF() {
        this.ESC().Text("C");
        return this;
    }

    Vt52CUB() {
        this.ESC().Text("D");
        return this;
    }

    Vt52SpecialGraphics() {
        this.ESC().Text("F");
    }

    Vt52Ascii() {
        this.ESC().Text("G");
    }

    Vt52Home() {
        this.ESC().Text("H");
        return this;
    }

    Vt52ReverseLineFeed() {
        this.ESC().Text("I");
        return this;
    }

    Vt52EraseToEndOfScreen() {
        this.ESC().Text("J");
        return this;
    }

    Vt52EraseToEndOfLine() {
        this.ESC().Text("K");
        return this;
    }

    Vt52DirectCursorAddress(row, column) {
        this.ESC().Text("Y" + String.fromCharCode(row + 31) + String.fromCharCode(column + 31));
        return this;
    }

    Vt52Identify() {
        this.ESC().Text("Z");
        return this;
    }

    Vt52EnterAlternateKeypad() {
        this.ESC().Text("=");
        return this;
    }

    Vt52ExitAlternateKeypad() {
        this.ESC().Text(">");
        return this;
    }

    Vt52EnterAnsiMode() {
        this.ESC().Text("<");
        return this;
    }
}
