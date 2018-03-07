namespace VtNetCoreUnitTests
{
    using System.Text;
    using VtNetCore.VirtualTerminal;
    using VtNetCore.XTermParser;
    using Xunit;

    public class LibvtermStateTabStops
    {
        private void Push(DataConsumer d, string s)
        {
            d.Push(Encoding.UTF8.GetBytes(s));
        }

        private bool IsCursor(VirtualTerminalController t, int row, int column)
        {
            return t.ViewPort.CursorPosition.Row == row && t.ViewPort.CursorPosition.Column == column;
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectInitialFirstTab =
            "abcdefghXjklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab ";
            
        public static readonly string ExpectInitialSecondTab =
            "abcdefghXjklmnopXrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab ";

        // !Initial
        // RESET
        // PUSH "\tX"
        //   putglyph 0x58 1 0,8
        // PUSH "\tX"
        //   putglyph 0x58 1 0,16
        //   ?cursor = 0,17
        [Fact]
        public void Initial()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "\tX"
            //   putglyph 0x58 1 0,8
            Push(d, "\tX");
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectInitialFirstTab, s);

            // PUSH "\tX"
            //   putglyph 0x58 1 0,16
            //   ?cursor = 0,17
            Push(d, "\tX");
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectInitialSecondTab, s);
            Assert.True(IsCursor(t, 0, 17));
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectHorizontalTabSet =
            "abcdXfghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab ";

        // !HTS
        // PUSH "\e[5G\eH"
        // PUSH "\e[G\tX"
        //   putglyph 0x58 1 0,4
        //   ?cursor = 0,5
        [Fact]
        public void HorizontalTabSet()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "\e[5G\eH"
            // PUSH "\e[G\tX"
            //   putglyph 0x58 1 0,4
            //   ?cursor = 0,5
            Push(d, "".CHA(5).HTS().CHA().T("\tX"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectHorizontalTabSet, s);
            Assert.True(IsCursor(t, 0, 5));
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectedTabClear =
            "abcdXfghijklmnopXrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab ";

        // !TBC 0
        // PUSH "\e[9G\e[g"
        // PUSH "\e[G\tX\tX"
        //   putglyph 0x58 1 0,4
        //   putglyph 0x58 1 0,16
        //   ?cursor = 0,17
        [Fact]
        public void TabClear()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "\e[5G\eH" // From TabSet
            // PUSH "\e[9G\e[g"
            // PUSH "\e[G\tX\tX"
            //   putglyph 0x58 1 0,4
            //   putglyph 0x58 1 0,16
            //   ?cursor = 0,17
            Push(d, "".CHA(5).HTS().CHA(9).TBC().CHA().T("\tX\tX"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectedTabClear, s);
            Assert.True(IsCursor(t, 0, 17));
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectedTabClearAll =
            "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwXyzabcdefghijklmnopqrstuvwxyzab ";

        // !TBC 3
        // PUSH "\e[3g\e[50G\eH\e[G"
        //   ?cursor = 0,0
        // PUSH "\tX"
        //   putglyph 0x58 1 0,49
        //   ?cursor = 0,50
        [Fact]
        public void TabClearAll()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "\e[3g\e[50G\eH\e[G"
            //   ?cursor = 0,0
            Push(d, "".TBC(3).CHA(50).HTS().CHA());
            Assert.True(IsCursor(t, 0, 0));

            // PUSH "\tX"
            //   putglyph 0x58 1 0,49
            //   ?cursor = 0,50
            Push(d, "\tX");
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectedTabClearAll, s);
            Assert.True(IsCursor(t, 0, 50));
        }

        // !Tabstops after resize
        // RESET
        // RESIZE 30,100
        // # Should be 100/8 = 12 tabstops
        // PUSH "\tX"
        //   putglyph 0x58 1 0,8
        // PUSH "\tX"
        //   putglyph 0x58 1 0,16
        // PUSH "\tX"
        //   putglyph 0x58 1 0,24
        // PUSH "\tX"
        //   putglyph 0x58 1 0,32
        // PUSH "\tX"
        //   putglyph 0x58 1 0,40
        // PUSH "\tX"
        //   putglyph 0x58 1 0,48
        // PUSH "\tX"
        //   putglyph 0x58 1 0,56
        // PUSH "\tX"
        //   putglyph 0x58 1 0,64
        // PUSH "\tX"
        //   putglyph 0x58 1 0,72
        // PUSH "\tX"
        //   putglyph 0x58 1 0,80
        // PUSH "\tX"
        //   putglyph 0x58 1 0,88
        // PUSH "\tX"
        //   putglyph 0x58 1 0,96
        //   ?cursor = 0,97

        //  "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000011"
        //  "00000000011111111112222222222333333333344444444445555555555666666666677777777778888888888999999999900"
        //  "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        //  "        |       |       |       |       |       |       |       |       |       |       |       |    "
        public static readonly string ExpectedTabStopsAfterResize =
            "abcdefghXjklmnopXrstuvwxXzabcdefXhijklmnXpqrstuvXxyzabcdXfghijklXnopqrstXvwxyzabXdefghijXlmnopqrXtuv ";

        [Fact]
        public void TabStopsAfterResize()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(100, 30);
            t.TestPatternScrollingDiagonalLower();

            // # Should be 100/8 = 12 tabstops
            // PUSH "\tX"
            //   putglyph 0x58 1 0,8
            // PUSH "\tX"
            //   putglyph 0x58 1 0,16
            // PUSH "\tX"
            //   putglyph 0x58 1 0,24
            // PUSH "\tX"
            //   putglyph 0x58 1 0,32
            // PUSH "\tX"
            //   putglyph 0x58 1 0,40
            // PUSH "\tX"
            //   putglyph 0x58 1 0,48
            // PUSH "\tX"
            //   putglyph 0x58 1 0,56
            // PUSH "\tX"
            //   putglyph 0x58 1 0,64
            // PUSH "\tX"
            //   putglyph 0x58 1 0,72
            // PUSH "\tX"
            //   putglyph 0x58 1 0,80
            // PUSH "\tX"
            //   putglyph 0x58 1 0,88
            // PUSH "\tX"
            //   putglyph 0x58 1 0,96
            //   ?cursor = 0,97
            Push(d, "\tX\tX\tX\tX\tX\tX\tX\tX\tX\tX\tX\tX");
            s = t.GetVisibleChars(0, 0, 101);
            Assert.Equal(ExpectedTabStopsAfterResize, s);
            Assert.True(IsCursor(t, 0, 97));
        }

        // TODO : Test behavior when tabs are already set
        // TODO : Test behavior when switching to and from 80/132 column modes
    }
}
