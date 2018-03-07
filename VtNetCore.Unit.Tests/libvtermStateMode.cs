namespace VtNetCoreUnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using VtNetCore.VirtualTerminal;
    using VtNetCore.XTermParser;
    using Xunit;

    public class LibvtermStateMode
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
        public static readonly string ExpectReplaceChar =
            "ABcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab ";

        public static readonly string ExpectInsertChar =
            "ABCABcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxy ";

        // !Insert/Replace Mode
        // RESET
        //   erase 0..25,0..80
        //   ?cursor = 0,0
        // PUSH "AC\e[DB"
        //   putglyph 0x41 1 0,0
        //   putglyph 0x43 1 0,1
        //   putglyph 0x42 1 0,1
        // PUSH "\e[4h"
        // PUSH "\e[G"
        // PUSH "AC\e[DB"
        //   moverect 0..1,0..79 -> 0..1,1..80
        //   erase 0..1,0..1
        //   putglyph 0x41 1 0,0
        //   moverect 0..1,1..79 -> 0..1,2..80
        //   erase 0..1,1..2
        //   putglyph 0x43 1 0,1
        //   moverect 0..1,1..79 -> 0..1,2..80
        //   erase 0..1,1..2
        //   putglyph 0x42 1 0,1
        [Fact]
        public void InsertCharacter()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "AC\e[DB"
            //   putglyph 0x41 1 0,0
            //   putglyph 0x43 1 0,1
            //   putglyph 0x42 1 0,1
            Push(d, "AC".CUB().T("B"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectReplaceChar, s);

            // PUSH "\e[4h"
            // PUSH "\e[G"
            // PUSH "AC\e[DB"
            //   moverect 0..1,0..79 -> 0..1,1..80
            //   erase 0..1,0..1
            //   putglyph 0x41 1 0,0
            //   moverect 0..1,1..79 -> 0..1,2..80
            //   erase 0..1,1..2
            //   putglyph 0x43 1 0,1
            //   moverect 0..1,1..79 -> 0..1,2..80
            //   erase 0..1,1..2
            //   putglyph 0x42 1 0,1
            Push(d, "".InsertMode().CHA().T("AC").CUB().T("B"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectInsertChar, s);
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectInsertBeforeCombine =
            "ABeCabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwx ";
        public static readonly string ExpectInsertAfterCombine =
            "ABe\u0301Cabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwx ";

        // !Insert mode only happens once for UTF-8 combining
        // PUSH "e"
        //   moverect 0..1,2..79 -> 0..1,3..80
        //   erase 0..1,2..3
        //   putglyph 0x65 1 0,2
        // PUSH "\xCC\x81"
        //   putglyph 0x65,0x301 1 0,2
        [Fact]
        public void InsertOnceForUtf8Combining()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "e"
            //   moverect 0..1,2..79 -> 0..1,3..80
            //   erase 0..1,2..3
            //   putglyph 0x65 1 0,2
            Push(d, "".InsertMode().CHA().T("AC").CUB().T("Be"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectInsertBeforeCombine, s);

            // PUSH "\xCC\x81"
            //   putglyph 0x65,0x301 1 0,2
            d.Push(new byte[] { 0xCC, 0x81 });
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectInsertAfterCombine, s);
        }

        // !Newline/Linefeed mode
        // RESET
        //   erase 0..25,0..80
        //   ?cursor = 0,0
        // PUSH "\e[5G\n"
        //   ?cursor = 1,4
        // PUSH "\e[20h"
        // PUSH "\e[5G\n"
        //   ?cursor = 2,0
        [Fact]
        public void NewlineLinefeedMode()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "\e[5G\n"
            //   ?cursor = 1,4
            Push(d, "".CHA(5).LF());
            s = t.GetVisibleChars(0, 0, 81);
            Assert.True(IsCursor(t, 1, 4));

            // PUSH "\e[20h"
            // PUSH "\e[5G\n"
            //   ?cursor = 2,0
            Push(d, "".CHA(5).AutomaticNewlineMode().LF());
            s = t.GetVisibleChars(0, 0, 81);
            Assert.True(IsCursor(t, 2, 0));
        }

        // !DEC origin mode
        // RESET
        //   erase 0..25,0..80
        //   ?cursor = 0,0
        // PUSH "\e[5;15r"
        // PUSH "\e[H"
        //   ?cursor = 0,0
        // PUSH "\e[3;3H"
        //   ?cursor = 2,2
        // PUSH "\e[?6h"
        // PUSH "\e[H"
        //   ?cursor = 4,0
        // PUSH "\e[3;3H"
        //   ?cursor = 6,2
        [Fact]
        public void DecOriginMode()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "\e[5;15r"
            // PUSH "\e[H"
            //   ?cursor = 0,0
            Push(d, "".STBM(5,15).CUP());
            Assert.True(IsCursor(t, 0, 0));

            // PUSH "\e[3;3H"
            //   ?cursor = 2,2
            Push(d, "".CUP(3,3));
            Assert.True(IsCursor(t, 2, 2));

            // PUSH "\e[?6h"
            // PUSH "\e[H"
            //   ?cursor = 4,0
            Push(d, "".EnableDECOM().CUP());
            Assert.True(IsCursor(t, 4, 0));

            // PUSH "\e[3;3H"
            //   ?cursor = 6,2
            Push(d, "".CUP(3,3));
            Assert.True(IsCursor(t, 6, 2));
        }

        // !DECRQM on DECOM
        // PUSH "\e[?6h"
        // PUSH "\e[?6\$p"
        //   output "\e[?6;1\$y"
        // PUSH "\e[?6l"
        // PUSH "\e[?6\$p"
        //   output "\e[?6;2\$y"
        [Fact]
        public void DECRQMOnDECOM()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);

            string toSend = "";
            t.SendData += (sender, args) =>
            {
                toSend = Encoding.ASCII.GetString(args.Data);
            };

            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "\e[?6h"
            // PUSH "\e[?6\$p"
            //   output "\e[?6;1\$y"
            Push(d, "".EnableDECOM().DecDECRQM(6));
            Assert.Equal("".CSI().Query().T("6;1$y"), toSend);

            // PUSH "\e[?6l"
            // PUSH "\e[?6\$p"
            //   output "\e[?6;2\$y"
            Push(d, "".DisableDECOM().DecDECRQM(6));
            Assert.Equal("".CSI().Query().T("6;2$y"), toSend);
        }

        // !Origin mode with DECSLRM
        // PUSH "\e[?6h"
        // PUSH "\e[?69h"
        // PUSH "\e[20;60s"
        // PUSH "\e[H"
        //   ?cursor = 4,19
        // 
        // PUSH "\e[?69l"
        [Fact]
        public void OriginModeWithDECSLRM()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "\e[5;15r"
            // PUSH "\e[H"
            //   ?cursor = 0,0
            Push(d, "".STBM(5, 15).CUP());
            Assert.True(IsCursor(t, 0, 0));

            // PUSH "\e[3;3H"
            //   ?cursor = 2,2
            Push(d, "".CUP(3, 3));
            Assert.True(IsCursor(t, 2, 2));

            // PUSH "\e[?6h"
            // PUSH "\e[H"
            //   ?cursor = 4,0
            Push(d, "".EnableDECOM().CUP());
            Assert.True(IsCursor(t, 4, 0));

            // PUSH "\e[3;3H"
            //   ?cursor = 6,2
            Push(d, "".CUP(3, 3));
            Assert.True(IsCursor(t, 6, 2));

            // PUSH "\e[?6h"
            // PUSH "\e[?69h"
            // PUSH "\e[20;60s"
            // PUSH "\e[H"
            //   ?cursor = 4,19
            // 
            // PUSH "\e[?69l"
            Push(d, "".EnableDECOM().EnableLRMM().LRMM(20,60).CUP());
            Assert.True(IsCursor(t, 4, 19));

            Push(d, "".DisableLRMM());
        }

        // !Origin mode bounds cursor to scrolling region
        // PUSH "\e[H"
        // PUSH "\e[10A"
        //   ?cursor = 4,0
        // PUSH "\e[20B"
        //   ?cursor = 14,0
        [Fact]
        public void OriginModeBoundsCursorToScrolling()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "\e[5;15r"
            // PUSH "\e[H"
            //   ?cursor = 0,0
            Push(d, "".STBM(5, 15).CUP());
            Assert.True(IsCursor(t, 0, 0));

            // PUSH "\e[?6h"
            // PUSH "\e[?69h"
            // PUSH "\e[20;60s"
            // PUSH "\e[H"
            //   ?cursor = 4,19
            // 
            // PUSH "\e[?69l"
            Push(d, "".EnableDECOM().EnableLRMM().LRMM(20, 60).CUP());
            Assert.True(IsCursor(t, 4, 19));

            // PUSH "\e[?69l"
            Push(d, "".DisableLRMM());

            // PUSH "\e[H"
            // PUSH "\e[10A"
            //   ?cursor = 4,0
            Push(d, "".CUP().CUU(10));
            Assert.True(IsCursor(t, 4, 0));

            // PUSH "\e[20B"
            //   ?cursor = 14,0
            Push(d, "".CUD(20));
            Assert.True(IsCursor(t, 14, 0));
        }

        // !Origin mode without scroll region
        // PUSH "\e[?6l"
        // PUSH "\e[r\e[?6h"
        //   ?cursor = 0,0
        [Fact]
        public void OriginModeWithoutScrollRegion()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "\e[5;15r"
            // PUSH "\e[H"
            //   ?cursor = 0,0
            Push(d, "".STBM(5, 15).CUP());
            Assert.True(IsCursor(t, 0, 0));

            // PUSH "\e[?6h"
            // PUSH "\e[?69h"
            // PUSH "\e[20;60s"
            // PUSH "\e[H"
            //   ?cursor = 4,19
            // 
            // PUSH "\e[?69l"
            Push(d, "".EnableDECOM().EnableLRMM().LRMM(20, 60).CUP());
            Assert.True(IsCursor(t, 4, 19));

            // PUSH "\e[?69l"
            Push(d, "".DisableLRMM());

            // PUSH "\e[H"
            // PUSH "\e[10A"
            //   ?cursor = 4,0
            Push(d, "".CUP().CUU(10));
            Assert.True(IsCursor(t, 4, 0));

            // PUSH "\e[20B"
            //   ?cursor = 14,0
            Push(d, "".CUD(20));
            Assert.True(IsCursor(t, 14, 0));

            // PUSH "\e[?6l"
            // PUSH "\e[r\e[?6h"
            //   ?cursor = 0,0
            Push(d, "".DisableDECOM().STBM().EnableDECOM());
            Assert.True(IsCursor(t, 0, 0));
        }
    }
}
