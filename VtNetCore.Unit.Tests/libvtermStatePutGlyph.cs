namespace VtNetCoreUnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using VtNetCore.VirtualTerminal;
    using VtNetCore.XTermParser;
    using Xunit;

    public class LibvtermStatePutGlyph
    {
        private void Push(DataConsumer d, string s)
        {
            d.Push(Encoding.UTF8.GetBytes(s));
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectLow =
            "ABCdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab ";

        // !Low
        // RESET
        // PUSH "ABC"
        //   putglyph 0x41 1 0,0
        //   putglyph 0x42 1 0,1
        //   putglyph 0x43 1 0,2
        [Fact]
        public void Low()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "ABC"
            //   putglyph 0x41 1 0,0
            //   putglyph 0x42 1 0,1
            //   putglyph 0x43 1 0,2
            Push(d, "ABC");
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectLow, s);
        }

        //  "0     0     0000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "1     2     3456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectSingleUtf8Char =
            "\u00c1\u00e9cdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab ";

        // !UTF-8 1 char
        // # U+00C1 = 0xC3 0x81  name: LATIN CAPITAL LETTER A WITH ACUTE
        // # U+00E9 = 0xC3 0xA9  name: LATIN SMALL LETTER E WITH ACUTE
        // RESET
        // PUSH "\xC3\x81\xC3\xA9"
        //   putglyph 0xc1 1 0,0
        //   putglyph 0xe9 1 0,1
        [Fact]
        public void SingleUtf8Char()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "\xC3\x81\xC3\xA9"
            //   putglyph 0xc1 1 0,0
            //   putglyph 0xe9 1 0,1
            d.Push(new byte[] { 0xC3, 0x81, 0xC3, 0xA9 });
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectSingleUtf8Char, s);
        }

        //  "0     00000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "1     23456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectUtf8SplitWrites =
            "\u00c1bcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab ";

        // !UTF-8 split writes
        // RESET
        // PUSH "\xC3"
        // PUSH "\x81"
        //   putglyph 0xc1 1 0,0
        [Fact]
        public void Utf8SplitWrites()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "\xC3"
            // PUSH "\x81"
            //   putglyph 0xc1 1 0,0
            d.Push(new byte[] { 0xC3 });
            d.Push(new byte[] { 0x81 });
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectUtf8SplitWrites, s);
        }

        //  "0     00000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "1     23456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectWideUtf8Char =
            "\uff10 cdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab ";

        // !UTF-8 wide char
        // # U+FF10 = 0xEF 0xBC 0x90  name: FULLWIDTH DIGIT ZERO
        // RESET
        // PUSH "\xEF\xBC\x90 "
        //   putglyph 0xff10 2 0,0
        //   putglyph 0x20 1 0,2
        [Fact]
        public void WideUtf8Char()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "\xEF\xBC\x90 "
            //   putglyph 0xff10 2 0,0
            //   putglyph 0x20 1 0,2
            d.Push(new byte[] { 0xEF, 0xBC, 0x90, (byte)' ' });
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectWideUtf8Char, s);
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectCombiningUtf8Chars =
            //"éZcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab "
            "e\u0301Zcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab ";

        // !UTF-8 combining chars
        // # U+0301 = 0xCC 0x81  name: COMBINING ACUTE
        // RESET
        // PUSH "e\xCC\x81Z"
        //   putglyph 0x65,0x301 1 0,0
        //   putglyph 0x5a 1 0,1
        [Fact]
        public void CombiningUtf8Chars()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "e\xCC\x81Z"
            //   putglyph 0x65,0x301 1 0,0
            //   putglyph 0x5a 1 0,1
            d.Push(new byte[] { (byte)'e', 0xCC, 0x81, (byte)'Z' });
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectCombiningUtf8Chars, s);
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectCombiningAcrossBuffersEOnly =
            //"éZcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab "
            "ebcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab ";

        public static readonly string ExpectDiacriticalEOnly =
            //"éZcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab "
            "e\u0301bcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab ";

        public static readonly string ExpectDoubleDiacriticalE =
            //"éZcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab "
            "e\u0301\u0302bcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab ";

        // !Combining across buffers
        // RESET
        // PUSH "e"
        //   putglyph 0x65 1 0,0
        // PUSH "\xCC\x81Z"
        //   putglyph 0x65,0x301 1 0,0
        //   putglyph 0x5a 1 0,1
        // 
        // RESET
        // PUSH "e"
        //   putglyph 0x65 1 0,0
        // PUSH "\xCC\x81"
        //   putglyph 0x65,0x301 1 0,0
        // PUSH "\xCC\x82"
        //   putglyph 0x65,0x301,0x302 1 0,0
        [Fact]
        public void CombiningAcrossBuffers()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // RESET
            // PUSH "e"
            //   putglyph 0x65 1 0,0
            d.Push(new byte[] { (byte)'e' });
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectCombiningAcrossBuffersEOnly, s);

            // PUSH "\xCC\x81Z"
            //   putglyph 0x65,0x301 1 0,0
            //   putglyph 0x5a 1 0,1
            d.Push(new byte[] { 0xCC, 0x81, (byte)'Z' });
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectCombiningUtf8Chars, s);

            // RESET
            // PUSH "e"
            //   putglyph 0x65 1 0,0
            t.FullReset();
            t.TestPatternScrollingDiagonalLower();
            d.Push(new byte[] { (byte)'e' });
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectCombiningAcrossBuffersEOnly, s);

            // PUSH "\xCC\x81"
            //   putglyph 0x65,0x301 1 0,0
            d.Push(new byte[] { 0xCC, 0x81 });
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDiacriticalEOnly, s);

            // PUSH "\xCC\x82"
            //   putglyph 0x65,0x301,0x302 1 0,0
            d.Push(new byte[] { 0xCC, 0x82 });
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectDoubleDiacriticalE, s);
        }

        // !DECSCA protected
        // RESET
        // PUSH "A\e[1\"qB\e[2\"qC"
        //   putglyph 0x41 1 0,0
        //   putglyph 0x42 1 0,1 prot
        //   putglyph 0x43 1 0,2

        [Fact]
        public void DECSCAProtected()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "A\e[1\"qB\e[2\"qC"
            //   putglyph 0x41 1 0,0
            //   putglyph 0x42 1 0,1 prot
            //   putglyph 0x43 1 0,2
            Push(d, "A".DECSCA(1).T("B").DECSCA(2).T("C"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectLow, s);
            Assert.False(t.IsProtected(0, 0));
            Assert.True(t.IsProtected(0, 1));
            Assert.False(t.IsProtected(0, 2));
        }
    }
}
