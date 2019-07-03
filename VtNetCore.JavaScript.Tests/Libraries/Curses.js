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
    }

    pushCsiCommand(p0, cmd) {
        this.CSI();
        if (p0 >= 0) {
            let i = 0;
            let s = p0.toString();
            while (i < s.length) {
                this.pushByte(s.charCodeAt(i));
                i++;
            }
        }
        this.pushByte(cmd.charCodeAt(0));
    }

    CSI() {
        this.ESC();
        this.pushByte('['.charCodeAt(0));
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

    ESC() {
        this.pushByte(0x1b);
        return this;
    }
}
