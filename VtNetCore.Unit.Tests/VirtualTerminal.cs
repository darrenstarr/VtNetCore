using System;
using System.Linq;
using VtNetCore.VirtualTerminal;
using VtNetCore.XTermParser;
using Xunit;

namespace VtNetCore.Unit.Tests
{
    public class VirtualTerminal
    {
        private void PushToTerminal(DataConsumer t, string s)
        {
            t.Push(s.Select(x => (byte)x).ToArray());
        }

        [Fact]
        public void Backspace()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);

            t.ResizeView(200, 100);
            PushToTerminal(d, "12345\u001b[D\u001b[D\b0");

            Assert.Equal("12045 ", t.GetVisibleChars(0, 0, 6));

            t = new VirtualTerminalController();
            d = new DataConsumer(t);
            t.ResizeView(200, 100);
            PushToTerminal(d, "12345\u001b[D\u001b[D\b");

            Assert.Equal("12345 ", t.GetVisibleChars(0, 0, 6));
        }

        [Fact]
        public void EraseToStartOfLine()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);

            t.ResizeView(200, 100);
            PushToTerminal(d, "12345\u001b[D\u001b[D\u001b[1K");

            Assert.Equal("    5 ", t.GetVisibleChars(0, 0, 6));
        }

        public readonly string ExpectedScreenAlignment =
            "EEEEE \n" +
            "EEEEE \n" +
            "EEEEE \n" +
            "EEEEE \n" +
            "EEEEE \n" +
            "      ";

        [Fact]
        public void ScreenAlignmentTest()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(5, 5);
            t.ScreenAlignmentTest();
            t.ResizeView(6, 6);

            Assert.Equal(ExpectedScreenAlignment, t.GetScreenText());
        }

        public readonly string ExpectedEraseAll =
            "     \n" +
            "     \n" +
            "     \n" +
            "     \n" +
            "     ";

        [Fact]
        public void EraseScreen()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);

            t.ResizeView(5, 5);
            t.ScreenAlignmentTest();
            t.EraseAll();

            Assert.Equal(ExpectedEraseAll, t.GetScreenText());

            t.ScreenAlignmentTest();
            PushToTerminal(d, "\u001B#8\u001B[2J");
            Assert.Equal(ExpectedEraseAll, t.GetScreenText());
        }

        public readonly string ExpectedEraseBelow =
            "EEEEE\n" +
            "EEE  \n" +
            "     \n" +
            "     \n" +
            "     ";

        [Fact]
        public void EraseBelow()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);

            t.ResizeView(5, 5);
            t.ScreenAlignmentTest();
            t.SetCursorPosition(4, 2);
            t.EraseBelow();

            Assert.Equal(ExpectedEraseBelow, t.GetScreenText());

            t.ScreenAlignmentTest();
            PushToTerminal(d, "\u001B#8\u001B[J");
            Assert.Equal(ExpectedEraseBelow, t.GetScreenText());

            t.ScreenAlignmentTest();
            PushToTerminal(d, "\u001B#8\u001B[;J");
            Assert.Equal(ExpectedEraseBelow, t.GetScreenText());

            t.ScreenAlignmentTest();
            PushToTerminal(d, "\u001B#8\u001B[0J");
            Assert.Equal(ExpectedEraseBelow, t.GetScreenText());
        }

        public readonly string ExpectedEraseAbove =
            "     \n" +
            "    E\n" +
            "EEEEE\n" +
            "EEEEE\n" +
            "EEEEE";

        [Fact]
        public void EraseAbove()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);

            t.ResizeView(5, 5);
            t.ScreenAlignmentTest();
            t.SetCursorPosition(4, 2);
            t.EraseAbove();

            Assert.Equal(ExpectedEraseAbove, t.GetScreenText());

            t.ScreenAlignmentTest();
            PushToTerminal(d, "\u001B#8\u001B[1J");
            Assert.Equal(ExpectedEraseAbove, t.GetScreenText());
        }

        public readonly string ExpectedScrollFull =
            "BBBBB\n" +
            "CCCCC\n" +
            "DDDDD\n" +
            "EEEEE\n" +
            "FFFF ";

        [Fact]
        public void ScrollFullDown()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);

            t.ResizeView(5, 5);
            t.ScreenAlignmentTest();

            PushToTerminal(d, "AAAAABBBBBCCCCCDDDDDEEEEEFFFF");
            Assert.Equal(ExpectedScrollFull, t.GetScreenText());
        }

        public readonly string ExpectedScrollRegionUp =
            "BBBBB\n" +
            "     \n" +
            "CCCCC\n" +
            "DDDDD\n" +
            "FFFF ";

        [Fact]
        public void ScrollRegionUp()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);

            t.ResizeView(5, 5);
            t.ScreenAlignmentTest();

            PushToTerminal(d, "AAAAABBBBBCCCCCDDDDDEEEEEFFFF");
            t.SetScrollingRegion(2, 4);
            t.EnableOriginMode(true);
            t.ReverseIndex();

            Assert.Equal(ExpectedScrollRegionUp, t.GetScreenText());
        }

        public readonly string ExpectedScrollRegionDown =
            "AAAAA\n" +
            "CCCCC\n" +
            "DDDDD\n" +
            "     \n" +
            "EEEEE";

        [Fact]
        public void ScrollRegionDown()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);

            t.ResizeView(5, 5);
            t.ScreenAlignmentTest();

            PushToTerminal(d, "AAAAABBBBBCCCCCDDDDDEEEEE");
            t.SetScrollingRegion(2, 4);
            t.EnableOriginMode(true);
            t.NewLine();
            t.NewLine();
            t.NewLine();

            string text = t.GetScreenText();
            Assert.Equal(ExpectedScrollRegionUp, text);
        }

        public readonly string ExpectedScrollUp =
            "     \n" +
            "BBBBB\n" +
            "CCCCC\n" +
            "DDDDD\n" +
            "EEEEE";

        [Fact]
        public void ScrollFullUp()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);

            t.ResizeView(5, 5);
            t.ScreenAlignmentTest();

            PushToTerminal(d, "AAAAABBBBBCCCCCDDDDDEEEEEFFFF");
            t.ReverseIndex();
            t.ReverseIndex();
            t.ReverseIndex();
            t.ReverseIndex();
            t.ReverseIndex();
            Assert.Equal(ExpectedScrollUp, t.GetScreenText());
        }

        public readonly string ExpectedRIND =
            "BBBBG\n" +
            "GGCCC\n" +
            "DDDDD\n" +
            "EEEEE\n" +
            "FFFF ";

        [Fact]
        public void ReverseIndex()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);

            t.ResizeView(5, 5);
            t.ScreenAlignmentTest();

            PushToTerminal(d, "AAAAABBBBBCCCCCDDDDDEEEEEFFFF\u001BM\u001BM\u001BM\u001BMGGG");
            Assert.Equal(ExpectedRIND, t.GetScreenText());
        }

        public readonly string Expected80ColumnMode =
            "01234567890123456789012345678901234567890123456789012345678901234567890123456789\n" +
            "01234567890123456789012345678901234567890123456789012345678901234567890123456789\n" +
            "                                                                                \n" +
            "                                                                                \n" +
            "                                                                                ";

        [Fact]
        public void EightyColumnMode()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);

            t.ResizeView(5, 5);
            PushToTerminal(d, "\u001b[?3l");

            Assert.Equal(80, t.Columns);
            Assert.Equal(5, t.Rows);

            for (var i = 0; i < 16; i++)
                PushToTerminal(d, "0123456789");

            Assert.Equal(Expected80ColumnMode, t.GetScreenText());
        }

        public readonly string Expected132ColumnMode =
            "012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901\n" +
            "2345678901234567890123456789                                                                                                        \n" +
            "                                                                                                                                    \n" +
            "                                                                                                                                    \n" +
            "                                                                                                                                    ";

        [Fact]
        public void OneThirtyTwoColumnMode()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);

            t.ResizeView(5, 5);
            PushToTerminal(d, "\u001b[?3h");

            Assert.Equal(132, t.Columns);
            Assert.Equal(5, t.Rows);

            for (var i = 0; i < 16; i++)
                PushToTerminal(d, "0123456789");

            Assert.Equal(Expected132ColumnMode, t.GetScreenText());
        }

        public readonly string ExpectedScrollInsertLineFullTop =
            "     \n" +
            "AAAAA\n" +
            "BBBBB\n" +
            "CCCCC\n" +
            "DDDDD";

        [Fact]
        public void InsertLineFullTop()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);

            t.ResizeView(5, 5);
            t.ScreenAlignmentTest();

            PushToTerminal(d, "AAAAABBBBBCCCCCDDDDDEEEEE");
            t.SetCursorPosition(1, 1);
            t.InsertLines(1);
            Assert.Equal(ExpectedScrollInsertLineFullTop, t.GetScreenText());
        }

    }
}
