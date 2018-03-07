namespace VtNetCoreUnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using VtNetCore.VirtualTerminal;
    using VtNetCore.XTermParser;
    using Xunit;

    public class LibvtermStateWrapping
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
        public static readonly string ExpectSeventyNinthColumn =
            "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvAAAAAb ";

        //!79th Column
        //PUSH "\e[75G"
        //PUSH "A"x5
        //  putglyph 0x41 1 0,74
        //  putglyph 0x41 1 0,75
        //  putglyph 0x41 1 0,76
        //  putglyph 0x41 1 0,77
        //  putglyph 0x41 1 0,78
        //  ?cursor = 0,79
        [Fact]
        public void SeventyNinthColumn()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            //PUSH "\e[75G"
            //PUSH "A"x5
            //  putglyph 0x41 1 0,74
            //  putglyph 0x41 1 0,75
            //  putglyph 0x41 1 0,76
            //  putglyph 0x41 1 0,77
            //  putglyph 0x41 1 0,78
            //  ?cursor = 0,79
            Push(d, "".CHA(75).T("AAAAA"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectSeventyNinthColumn, s);
            Assert.True(IsCursor(t, 0, 79));
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectEightiethPhantom =
            "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzaA ";

        //!80th Column Phantom
        //PUSH "A"
        //  putglyph 0x41 1 0,79
        //  ?cursor = 0,79
        [Fact]
        public void EightiethPhantom()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            //PUSH "\e[79G"
            //PUSH "A"
            //  putglyph 0x41 1 0,79
            //  ?cursor = 0,79
            Push(d, "".CHA(80).T("A"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectEightiethPhantom, s);
            Assert.True(IsCursor(t, 0, 79));
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectLineWraparound1 =
            "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzaA ";
        public static readonly string ExpectLineWraparound2 =
            "Babcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyza ";

        //!Line Wraparound
        //PUSH "B"
        //  putglyph 0x42 1 1,0
        //  ?cursor = 1,1
        [Fact]
        public void LineWraparound()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            //PUSH "\e[79G"
            //PUSH "A"
            //  putglyph 0x41 1 0,79
            //  ?cursor = 0,79
            //PUSH "B"
            //  putglyph 0x42 1 1,0
            //  ?cursor = 1,1
            Push(d, "".CHA(80).T("AB"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectLineWraparound1, s);
            s = t.GetVisibleChars(0, 1, 81);
            Assert.Equal(ExpectLineWraparound2, s);
            Assert.True(IsCursor(t, 1, 1));
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectLineWraparoundCombined1 =
            "babcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxBBB ";
        public static readonly string ExpectLineWraparoundCombined2 =
            "CCabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz ";

        //!Line Wraparound during combined write
        //PUSH "\e[78G"
        //PUSH "BBBCC"
        //  putglyph 0x42 1 1,77
        //  putglyph 0x42 1 1,78
        //  putglyph 0x42 1 1,79
        //  putglyph 0x43 1 2,0
        //  putglyph 0x43 1 2,1
        //  ?cursor = 2,2
        [Fact]
        public void LineWraparoundCombined()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            //PUSH "\e[78G"
            //PUSH "BBBCC"
            //  putglyph 0x42 1 1,77
            //  putglyph 0x42 1 1,78
            //  putglyph 0x42 1 1,79
            //  putglyph 0x43 1 2,0
            //  putglyph 0x43 1 2,1
            //  ?cursor = 2,2
            Push(d, "".CUP(2, 78).T("BBBCC"));
            s = t.GetVisibleChars(0, 1, 81);
            Assert.Equal(ExpectLineWraparoundCombined1, s);
            s = t.GetVisibleChars(0, 2, 81);
            Assert.Equal(ExpectLineWraparoundCombined2, s);
            Assert.True(IsCursor(t, 2, 2));
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectDecAutoWrapMode1 =
            "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvDDDDDD ";
        public static readonly string ExpectDecAutoWrapMode2 =
            "babcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyza ";

        //!DEC Auto Wrap Mode
        //RESET
        //PUSH "\e[?7l"
        //PUSH "\e[75G"
        //PUSH "D"x6
        //  putglyph 0x44 1 0,74
        //  putglyph 0x44 1 0,75
        //  putglyph 0x44 1 0,76
        //  putglyph 0x44 1 0,77
        //  putglyph 0x44 1 0,78
        //  putglyph 0x44 1 0,79
        //  ?cursor = 0,79
        //PUSH "D"
        //  putglyph 0x44 1 0,79
        //  ?cursor = 0,79
        //PUSH "\e[?7h"
        [Fact]
        public void DecAutoWrapMode()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            //PUSH "\e[?7l"
            //PUSH "\e[75G"
            //PUSH "D"x6
            //  putglyph 0x44 1 0,74
            //  putglyph 0x44 1 0,75
            //  putglyph 0x44 1 0,76
            //  putglyph 0x44 1 0,77
            //  putglyph 0x44 1 0,78
            //  putglyph 0x44 1 0,79
            //  ?cursor = 0,79
            Push(d, "".DisableWrapAround().CHA(75).T("DDDDDD"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDecAutoWrapMode1, s);
            s = t.GetVisibleChars(0, 1, 81);
            Assert.Equal(ExpectDecAutoWrapMode2, s);
            Assert.True(IsCursor(t, 0, 79));

            //PUSH "D"
            //  putglyph 0x44 1 0,79
            //  ?cursor = 0,79
            //PUSH "\e[?7h"
            Push(d, "D".EnableWrapAround());
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDecAutoWrapMode1, s);
            s = t.GetVisibleChars(0, 1, 81);
            Assert.Equal(ExpectDecAutoWrapMode2, s);
            Assert.True(IsCursor(t, 0, 79));
        }

        public static readonly string ExpectEightiethWrapAroundBefore =
            "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab" + "\n" +
            "babcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyza" + "\n" +
            "cbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" + "\n" +
            "dcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxy" + "\n" +
            "edcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwx" + "\n" +
            "fedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvw" + "\n" +
            "gfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuv" + "\n" +
            "hgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstu" + "\n" +
            "ihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrst" + "\n" +
            "jihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrs" + "\n" +
            "kjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqr" + "\n" +
            "lkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopq" + "\n" +
            "mlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnop" + "\n" +
            "nmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmno" + "\n" +
            "onmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmn" + "\n" +
            "ponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklm" + "\n" +
            "qponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijkl" + "\n" +
            "rqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijk" + "\n" +
            "srqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghij" + "\n" +
            "tsrqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghi" + "\n" +
            "utsrqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefgh" + "\n" +
            "vutsrqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefg" + "\n" +
            "wvutsrqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdef" + "\n" +
            "xwvutsrqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcde" + "\n" +
            "yxwvutsrqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzaABC";

        public static readonly string ExpectEightiethWrapAroundAfter =
            "babcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyza" + "\n" +
            "cbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" + "\n" +
            "dcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxy" + "\n" +
            "edcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwx" + "\n" +
            "fedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvw" + "\n" +
            "gfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuv" + "\n" +
            "hgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstu" + "\n" +
            "ihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrst" + "\n" +
            "jihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrs" + "\n" +
            "kjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqr" + "\n" +
            "lkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopq" + "\n" +
            "mlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnop" + "\n" +
            "nmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmno" + "\n" +
            "onmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmn" + "\n" +
            "ponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklm" + "\n" +
            "qponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijkl" + "\n" +
            "rqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijk" + "\n" +
            "srqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghij" + "\n" +
            "tsrqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghi" + "\n" +
            "utsrqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefgh" + "\n" +
            "vutsrqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefg" + "\n" +
            "wvutsrqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdef" + "\n" +
            "xwvutsrqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcde" + "\n" +
            "yxwvutsrqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzaABC" + "\n" +
            "D                                                                               ";

        //!80th column causes linefeed on wraparound
        //PUSH "\e[25;78HABC"
        //  putglyph 0x41 1 24,77
        //  putglyph 0x42 1 24,78
        //  putglyph 0x43 1 24,79
        //  ?cursor = 24,79
        //PUSH "D"
        //  moverect 1..25,0..80 -> 0..24,0..80
        //  putglyph 0x44 1 24,0
        [Fact]
        void EightiethWrapAround()
        {
            var s = string.Empty;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            //PUSH "\e[25;78HABC"
            //  putglyph 0x41 1 24,77
            //  putglyph 0x42 1 24,78
            //  putglyph 0x43 1 24,79
            //  ?cursor = 24,79
            Push(d, "".CUP(25,78).T("ABC"));
            Assert.True(IsCursor(t, 24, 79));
            s = t.GetScreenText();
            Assert.Equal(ExpectEightiethWrapAroundBefore, s);

            //PUSH "D"
            //  moverect 1..25,0..80 -> 0..24,0..80
            //  putglyph 0x44 1 24,0
            Push(d, "".T("D"));
            s = t.GetScreenText();
            Assert.Equal(ExpectEightiethWrapAroundAfter, s);
        }

        public static readonly string ExpectEightiethCursorMoveCancelsPhantom =
            "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab" + "\n" +
            "babcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyza" + "\n" +
            "cbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" + "\n" +
            "dcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxy" + "\n" +
            "edcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwx" + "\n" +
            "fedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvw" + "\n" +
            "gfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuv" + "\n" +
            "hgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstu" + "\n" +
            "ihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrst" + "\n" +
            "jihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrs" + "\n" +
            "kjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqr" + "\n" +
            "lkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopq" + "\n" +
            "mlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnop" + "\n" +
            "nmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmno" + "\n" +
            "onmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmn" + "\n" +
            "ponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklm" + "\n" +
            "qponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijkl" + "\n" +
            "rqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijk" + "\n" +
            "srqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghij" + "\n" +
            "tsrqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghi" + "\n" +
            "utsrqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefgh" + "\n" +
            "vutsrqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefg" + "\n" +
            "wvutsrqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdef" + "\n" +
            "xwvutsrqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcde" + "\n" +
            "DxwvutsrqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzaABC";

        //!80th column phantom linefeed phantom cancelled by explicit cursor move
        //PUSH "\e[25;78HABC"
        //  putglyph 0x41 1 24,77
        //  putglyph 0x42 1 24,78
        //  putglyph 0x43 1 24,79
        //  ?cursor = 24,79
        //PUSH "\e[25;1HD"
        //  putglyph 0x44 1 24,0
        [Fact]
        void EightiethCursorMoveCancelsPhantom()
        {
            var s = string.Empty;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            //PUSH "\e[25;78HABC"
            //  putglyph 0x41 1 24,77
            //  putglyph 0x42 1 24,78
            //  putglyph 0x43 1 24,79
            //  ?cursor = 24,79
            Push(d, "".CUP(25, 78).T("ABC"));
            Assert.True(IsCursor(t, 24, 79));
            s = t.GetScreenText();
            Assert.Equal(ExpectEightiethWrapAroundBefore, s);

            //PUSH "\e[25;1HD"
            //  putglyph 0x44 1 24,0
            Push(d, "".CUP(25,1).T("D"));
            s = t.GetScreenText();
            Assert.Equal(ExpectEightiethCursorMoveCancelsPhantom, s);
        }
    }
}
