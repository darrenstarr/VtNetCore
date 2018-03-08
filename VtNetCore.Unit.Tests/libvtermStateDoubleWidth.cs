namespace VtNetCoreUnitTests
{
    using System.Text;
    using VtNetCore.VirtualTerminal;
    using VtNetCore.XTermParser;
    using Xunit;

    public class LibvtermStateDoubleWidth
    {
        private void Push(DataConsumer d, string s)
        {
            d.Push(Encoding.UTF8.GetBytes(s));
        }

        private bool IsCursor(VirtualTerminalController t, int row, int column)
        {
            return t.ViewPort.CursorPosition.Row == row && t.ViewPort.CursorPosition.Column == column;
        }

        private bool IsDoubleWideLine(VirtualTerminalController t, int row)
        {
            var line = t.ViewPort.GetVisibleLine(row);
            if (line == null)
                return false;

            return line.DoubleWidth;
        }

        private bool IsDoubleHeightTop(VirtualTerminalController t, int row)
        {
            var line = t.ViewPort.GetVisibleLine(row);
            if (line == null)
                return false;

            return line.DoubleHeightTop;
        }

        private bool IsDoubleHeightBottom(VirtualTerminalController t, int row)
        {
            var line = t.ViewPort.GetVisibleLine(row);
            if (line == null)
                return false;

            return line.DoubleHeightBottom;
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectSingleWidthSingleHeight =
            "Hellofghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab ";

        // !Single Width, Single Height
        // RESET
        // PUSH "\e#5"
        // PUSH "Hello"
        //   putglyph 0x48 1 0,0
        //   putglyph 0x65 1 0,1
        //   putglyph 0x6c 1 0,2
        //   putglyph 0x6c 1 0,3
        //   putglyph 0x6f 1 0,4
        [Fact]
        public void SingleWidthSingleHeight()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "\e#5"
            // PUSH "Hello"
            //   putglyph 0x48 1 0,0
            //   putglyph 0x65 1 0,1
            //   putglyph 0x6c 1 0,2
            //   putglyph 0x6c 1 0,3
            //   putglyph 0x6f 1 0,4
            Push(d, "".DECSWL().T("Hello"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectSingleWidthSingleHeight, s);
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectDoubleWidthSingleHeight =
            "Hellofghijklmnopqrstuvwxyzabcdefghijklmn                                         ";
        public static readonly string ExpectDoubleWidthSingleHeightWrap1 =
            "HellofghijklmnopqrstuvwxyzabcdefghijklmA                                         ";
        public static readonly string ExpectDoubleWidthSingleHeightWrap2 =
            "Babcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyza ";

        // !Double Width, Single Height
        // RESET
        // PUSH "\e#6"
        // PUSH "Hello"
        //   putglyph 0x48 1 0,0 dwl
        //   putglyph 0x65 1 0,1 dwl
        //   putglyph 0x6c 1 0,2 dwl
        //   putglyph 0x6c 1 0,3 dwl
        //   putglyph 0x6f 1 0,4 dwl
        //   ? cursor = 0,5
        // PUSH "\e[40GAB"
        //   putglyph 0x41 1 0,39 dwl
        //   putglyph 0x42 1 1,0
        //   ?cursor = 1,1
        [Fact]
        public void DoubleWidthSingleHeight()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "\e#6"
            // PUSH "Hello"
            //   putglyph 0x48 1 0,0 dwl
            //   putglyph 0x65 1 0,1 dwl
            //   putglyph 0x6c 1 0,2 dwl
            //   putglyph 0x6c 1 0,3 dwl
            //   putglyph 0x6f 1 0,4 dwl
            //   ? cursor = 0,5
            Push(d, "".DECDWL().T("Hello"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDoubleWidthSingleHeight, s);
            Assert.True(IsDoubleWideLine(t, 0));
            Assert.True(IsCursor(t, 0, 5));

            // PUSH "\e[40GAB"
            //   putglyph 0x41 1 0,39 dwl
            //   putglyph 0x42 1 1,0
            //   ?cursor = 1,1
            Push(d, "".CHA(40).T("AB"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDoubleWidthSingleHeightWrap1, s);
            Assert.True(IsDoubleWideLine(t, 0));
            s = t.GetVisibleChars(0, 1, 81);
            Assert.Equal(ExpectDoubleWidthSingleHeightWrap2, s);
            Assert.False(IsDoubleWideLine(t, 1));
            Assert.True(IsCursor(t, 1, 1));
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectDoubleHeightBottom =
            "Helloefghijklmnopqrstuvwxyzabcdefghijklm                                         ";

        // !Double Height
        // RESET
        // PUSH "\e#3"
        // PUSH "Hello"
        //   putglyph 0x48 1 0,0 dwl dhl-top
        //   putglyph 0x65 1 0,1 dwl dhl-top
        //   putglyph 0x6c 1 0,2 dwl dhl-top
        //   putglyph 0x6c 1 0,3 dwl dhl-top
        //   putglyph 0x6f 1 0,4 dwl dhl-top
        //   ? cursor = 0,5
        // PUSH "\r\n\e#4"
        // PUSH "Hello"
        //   putglyph 0x48 1 1,0 dwl dhl-bottom
        //   putglyph 0x65 1 1,1 dwl dhl-bottom
        //   putglyph 0x6c 1 1,2 dwl dhl-bottom
        //   putglyph 0x6c 1 1,3 dwl dhl-bottom
        //   putglyph 0x6f 1 1,4 dwl dhl-bottom
        //   ? cursor = 1,5
        [Fact]
        public void DoubleHeight()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "\e#3"
            // PUSH "Hello"
            //   putglyph 0x48 1 0,0 dwl dhl-top
            //   putglyph 0x65 1 0,1 dwl dhl-top
            //   putglyph 0x6c 1 0,2 dwl dhl-top
            //   putglyph 0x6c 1 0,3 dwl dhl-top
            //   putglyph 0x6f 1 0,4 dwl dhl-top
            //   ? cursor = 0,5
            Push(d, "".DECDHLTop().T("Hello"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDoubleWidthSingleHeight, s);
            Assert.True(IsDoubleWideLine(t, 0));
            Assert.True(IsDoubleHeightTop(t, 0));
            Assert.True(IsCursor(t, 0, 5));

            // PUSH "\r\n\e#4"
            // PUSH "Hello"
            //   putglyph 0x48 1 1,0 dwl dhl-bottom
            //   putglyph 0x65 1 1,1 dwl dhl-bottom
            //   putglyph 0x6c 1 1,2 dwl dhl-bottom
            //   putglyph 0x6c 1 1,3 dwl dhl-bottom
            //   putglyph 0x6f 1 1,4 dwl dhl-bottom
            //   ? cursor = 1,5
            Push(d, "\r\n".DECDHLBottom().T("Hello"));
            s = t.GetVisibleChars(0, 1, 81);
            Assert.Equal(ExpectDoubleHeightBottom, s);
            Assert.True(IsDoubleWideLine(t, 1));
            Assert.True(IsDoubleHeightBottom(t, 1));
            Assert.True(IsCursor(t, 1, 5));
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectDoubleWidthScrolling1 =
            "ABCqponmlkjihgfedcbabcdefghijklmnopqrstu                                         ";
        public static readonly string ExpectDoubleWidthScrolling2 =
            "ABCDEonmlkjihgfedcbabcdefghijklmnopqrstu                                         ";
        public static readonly string ExpectDoubleWidthScrolling3 =
            "ABCDEFGmlkjihgfedcbabcdefghijklmnopqrstu                                         ";

        // !Double Width scrolling
        // RESET
        // PUSH "\e[20H\e#6ABC"
        //   putglyph 0x41 1 19,0 dwl
        //   putglyph 0x42 1 19,1 dwl
        //   putglyph 0x43 1 19,2 dwl
        // PUSH "\e[25H\n"
        // PUSH "\e[19;4HDE"
        //   putglyph 0x44 1 18,3 dwl
        //   putglyph 0x45 1 18,4 dwl
        // PUSH "\e[H\eM"
        // PUSH "\e[20;6HFG"
        //   putglyph 0x46 1 19,5 dwl
        //   putglyph 0x47 1 19,6 dwl
        [Fact]
        public void DoubleWidthScrolling()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "\e[20H\e#6ABC"
            //   putglyph 0x41 1 19,0 dwl
            //   putglyph 0x42 1 19,1 dwl
            //   putglyph 0x43 1 19,2 dwl
            Push(d, "".CUP(20).DECDWL().T("ABC"));
            s = t.GetVisibleChars(0, 19, 81);
            Assert.Equal(ExpectDoubleWidthScrolling1, s);
            Assert.True(IsDoubleWideLine(t, 19));

            // PUSH "\e[25H\n"
            // PUSH "\e[19;4HDE"
            //   putglyph 0x44 1 18,3 dwl
            //   putglyph 0x45 1 18,4 dwl
            Push(d, "".CUP(25).LF().CUP(19,4).T("DE"));
            s = t.GetVisibleChars(0, 18, 81);
            Assert.Equal(ExpectDoubleWidthScrolling2, s);
            Assert.True(IsDoubleWideLine(t, 18));

            // PUSH "\e[H\eM"
            // PUSH "\e[20;6HFG"
            //   putglyph 0x46 1 19,5 dwl
            //   putglyph 0x47 1 19,6 dwl
            Push(d, "".CUP().RI().CUP(20,6).T("FG"));
            s = t.GetVisibleChars(0, 19, 81);
            Assert.Equal(ExpectDoubleWidthScrolling3, s);
            Assert.True(IsDoubleWideLine(t, 19));
        }

        // TODO : Test double wide tabs
    }
}
