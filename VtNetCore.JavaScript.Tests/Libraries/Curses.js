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

    CUP(row=-1, column=-1) {
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
}
