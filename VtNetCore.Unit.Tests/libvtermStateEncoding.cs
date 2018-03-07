namespace VtNetCoreUnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using VtNetCore.VirtualTerminal;
    using VtNetCore.XTermParser;
    using Xunit;

    public class LibvtermStateEncoding
    {
        private void Push(DataConsumer d, string s)
        {
            d.Push(Encoding.UTF8.GetBytes(s));
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectDefault =
            "#bcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab ";

        // !Default
        // RESET
        // PUSH "#"
        //   putglyph 0x23 1 0,0
        [Fact]
        public void Default()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "#"
            //   putglyph 0x23 1 0,0
            Push(d, "#");
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDefault, s);
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectDesignateG0UK =
            "\u00a3bcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab ";

        // !Designate G0 = UK
        // RESET
        // PUSH "\e(A"
        // PUSH "#"
        //   putglyph 0x00a3 1 0,0
        [Fact]
        public void DesignateG0UK()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "\e(A"
            // PUSH "#"
            //   putglyph 0x00a3 1 0,0
            Push(d, "".DesignateG0('A').T("#"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDesignateG0UK, s);
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectDesignateG0DecDrawing =
            "\u2592bcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab ";

        // !Designate G0 = DEC drawing
        //   RESET
        // PUSH "\e(0"
        // PUSH "a"
        //   putglyph 0x2592 1 0,0
        [Fact]
        public void DesignateG0DecDrawing()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "\e(0"
            // PUSH "a"
            //   putglyph 0x2592 1 0,0
            Push(d, "".DesignateG0('0').T("a"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDesignateG0DecDrawing, s);
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectDesignate =
            "aBCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZAB ";

        public static readonly string ExpectDesignateAndShift =
            "a\u2592CDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZAB ";

        public static readonly string ExpectDesignateAndShiftBack =
            "a\u2592aDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZAB ";

        // !Designate G1 + LS1
        // RESET
        // PUSH "\e)0"
        // PUSH "a"
        //   putglyph 0x61 1 0,0
        // PUSH "\x0e"
        // PUSH "a"
        //   putglyph 0x2592 1 0,1
        // !LS0
        // PUSH "\x0f"
        // PUSH "a"
        //   putglyph 0x61 1 0,2
        [Fact]
        public void DesignateG1AndLs1()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalUpper();

            // PUSH "\e)0"
            // PUSH "a"
            //   putglyph 0x61 1 0,0
            Push(d, "".DesignateG1('0').T("a"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDesignate, s);

            // PUSH "\x0e"
            // PUSH "a"
            //   putglyph 0x2592 1 0,1
            Push(d, "".ShiftOut().T("a"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDesignateAndShift, s);

            // !LS0
            // PUSH "\x0f"
            // PUSH "a"
            //   putglyph 0x61 1 0,2
            Push(d, "".ShiftIn().T("a"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDesignateAndShiftBack, s);
        }

        // !Designate G2 + LS2
        // PUSH "\e*0"
        // PUSH "a"
        //   putglyph 0x61 1 0,3
        // PUSH "\en"
        // PUSH "a"
        //   putglyph 0x2592 1 0,4
        // PUSH "\x0f"
        // PUSH "a"
        //   putglyph 0x61 1 0,5
        [Fact]
        public void DesignateG2AndLs2()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalUpper();

            // PUSH "\e*0"
            // PUSH "a"
            //   putglyph 0x61 1 0,3
            Push(d, "".DesignateG2('0').T("a"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDesignate, s);

            // PUSH "\en"
            // PUSH "a"
            //   putglyph 0x2592 1 0,4
            Push(d, "".LS2().T("a"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDesignateAndShift, s);

            // PUSH "\x0f"
            // PUSH "a"
            //   putglyph 0x61 1 0,5
            Push(d, "".ShiftIn().T("a"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDesignateAndShiftBack, s);
        }

        // !Designate G3 + LS3
        // PUSH "\e+0"
        // PUSH "a"
        //   putglyph 0x61 1 0,6
        // PUSH "\eo"
        // PUSH "a"
        //   putglyph 0x2592 1 0,7
        // PUSH "\x0f"
        // PUSH "a"
        //   putglyph 0x61 1 0,8
        [Fact]
        public void DesignateG2AndLs3()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalUpper();

            // PUSH "\e*0"
            // PUSH "a"
            //   putglyph 0x61 1 0,3
            Push(d, "".DesignateG3('0').T("a"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDesignate, s);

            // PUSH "\en"
            // PUSH "a"
            //   putglyph 0x2592 1 0,4
            Push(d, "".LS3().T("a"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDesignateAndShift, s);

            // PUSH "\x0f"
            // PUSH "a"
            //   putglyph 0x61 1 0,5
            Push(d, "".ShiftIn().T("a"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDesignateAndShiftBack, s);
        }

        // !SS2
        // PUSH "a\x{8e}aa"
        //   putglyph 0x61 1 0,9
        //   putglyph 0x2592 1 0,10
        //   putglyph 0x61 1 0,11
        [Fact]
        public void SS2()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalUpper();

            // PUSH "a\x{8e}aa"
            //   putglyph 0x61 1 0,9
            //   putglyph 0x2592 1 0,10
            //   putglyph 0x61 1 0,11
            Push(d, "".DesignateG2('0').T("a").SS2('a').T("a"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDesignateAndShiftBack, s);
        }

        // !SS3
        // PUSH "a\x{8f}aa"
        //   putglyph 0x61 1 0,12
        //   putglyph 0x2592 1 0,13
        //   putglyph 0x61 1 0,14
        [Fact]
        public void SS3()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalUpper();

            // PUSH "a\x{8f}aa"
            //   putglyph 0x61 1 0,12
            //   putglyph 0x2592 1 0,13
            //   putglyph 0x61 1 0,14
            Push(d, "".DesignateG3('0').T("a").SS3('a').T("a"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDesignateAndShiftBack, s);
        }

        // !LS1R
        // RESET
        // PUSH "\e~"
        // PUSH "\xe1"
        //   putglyph 0x61 1 0,0
        // PUSH "\e)0"
        // PUSH "\xe1"
        //   putglyph 0x2592 1 0,1
        [Fact]
        public void LS1R()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalUpper();

            // PUSH "\e~"
            // PUSH "\xe1"
            //   putglyph 0x61 1 0,0
            Push(d, "".LS1R());
            d.Push(new byte[] { 0xe1 });
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDesignate, s);

            //// PUSH "\e)0"
            //// PUSH "\xe1"
            ////   putglyph 0x2592 1 0,1
            Push(d, "".DesignateG1('0'));
            d.Push(new byte[] { 0xe1 });
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDesignateAndShift, s);
        }

        // !LS2R
        // RESET
        // PUSH "\e}"
        // PUSH "\xe1"
        //   putglyph 0x61 1 0,0
        // PUSH "\e*0"
        // PUSH "\xe1"
        //   putglyph 0x2592 1 0,1
        [Fact]
        public void LS2R()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalUpper();

            // PUSH "\e}"
            // PUSH "\xe1"
            //   putglyph 0x61 1 0,0
            Push(d, "".LS2R());
            d.Push(new byte[] { 0xe1 });
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDesignate, s);

            // PUSH "\e*0"
            // PUSH "\xe1"
            //   putglyph 0x2592 1 0,1
            Push(d, "".DesignateG2('0'));
            d.Push(new byte[] { 0xe1 });
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDesignateAndShift, s);
        }

        // !LS3R
        // RESET
        // PUSH "\e|"
        // PUSH "\xe1"
        //   putglyph 0x61 1 0,0
        // PUSH "\e+0"
        // PUSH "\xe1"
        //   putglyph 0x2592 1 0,1
        [Fact]
        public void LS3R()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalUpper();

            // PUSH "\e|"
            // PUSH "\xe1"
            //   putglyph 0x61 1 0,0
            Push(d, "".LS3R());
            d.Push(new byte[] { 0xe1 });
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDesignate, s);

            // PUSH "\e+0"
            // PUSH "\xe1"
            //   putglyph 0x2592 1 0,1
            Push(d, "".DesignateG3('0'));
            d.Push(new byte[] { 0xe1 });
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDesignateAndShift, s);
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectMixedASCIIAndUtf8 =
            "AB\u0108DEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZAB ";

        // UTF8 1
        // 
        // !Mixed US-ASCII and UTF-8
        // # U+0108 == 0xc4 0x88
        // RESET
        // PUSH "\e(B"
        // PUSH "AB\xc4\x88D"
        //   putglyph 0x0041 1 0,0
        //   putglyph 0x0042 1 0,1
        //   putglyph 0x0108 1 0,2
        //   putglyph 0x0044 1 0,3
        [Fact]
        public void MixedASCIIAndUtf8()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalUpper();

            // RESET
            // PUSH "\e(B"
            // PUSH "AB\xc4\x88D"
            //   putglyph 0x0041 1 0,0
            //   putglyph 0x0042 1 0,1
            //   putglyph 0x0108 1 0,2
            //   putglyph 0x0044 1 0,3
            Push(d, "".DesignateG0('B').T("AB"));
            d.Push(new byte[] { 0xc4, 0x88 });
            Push(d, "D");
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectMixedASCIIAndUtf8, s);
        }
    }
}
