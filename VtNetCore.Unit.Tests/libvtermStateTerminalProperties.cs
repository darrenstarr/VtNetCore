namespace VtNetCoreUnitTests
{
    using System.Text;
    using VtNetCore.VirtualTerminal;
    using VtNetCore.XTermParser;
    using Xunit;

    public class LibvtermStateTerminalProperties
    {
        private void Push(DataConsumer d, string s)
        {
            d.Push(Encoding.UTF8.GetBytes(s));
        }

        // !Cursor visibility
        // PUSH "\e[?25h"
        //   settermprop 1 true
        // PUSH "\e[?25\$p"
        //   output "\e[?25;1\$y"
        // PUSH "\e[?25l"
        //   settermprop 1 false
        // PUSH "\e[?25\$p"
        //   output "\e[?25;2\$y"
        [Fact]
        public void CursorVisibility()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            string toSend = "";
            t.SendData += (sender, args) =>
            {
                toSend = Encoding.ASCII.GetString(args.Data);
            };

            // PUSH "\e[?25h"
            //   settermprop 1 true
            // PUSH "\e[?25\$p"
            //   output "\e[?25;1\$y"
            Push(d, "".EnableDECTCEM().DecDECRQM(25));
            Assert.Equal("".CSI().Query().T("25;1$y"), toSend);

            // PUSH "\e[?25l"
            //   settermprop 1 false
            // PUSH "\e[?25\$p"
            //   output "\e[?25;2\$y"
            Push(d, "".DisableDECTCEM().DecDECRQM(25));
            Assert.Equal("".CSI().Query().T("25;2$y"), toSend);
        }

        // !Cursor blink
        // PUSH "\e[?12h"
        //   settermprop 2 true
        // PUSH "\e[?12\$p"
        //   output "\e[?12;1\$y"
        // PUSH "\e[?12l"
        //   settermprop 2 false
        // PUSH "\e[?12\$p"
        //   output "\e[?12;2\$y"
        [Fact]
        public void CursorBlink()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            string toSend = "";
            t.SendData += (sender, args) =>
            {
                toSend = Encoding.ASCII.GetString(args.Data);
            };

            // PUSH "\e[?12h"
            //   settermprop 2 true
            // PUSH "\e[?12\$p"
            //   output "\e[?12;1\$y"
            Push(d, "".StartBlinkingCursor().DecDECRQM(12));
            Assert.Equal("".CSI().Query().T("12;1$y"), toSend);

            // PUSH "\e[?12l"
            //   settermprop 2 false
            // PUSH "\e[?12\$p"
            //   output "\e[?12;2\$y"
            Push(d, "".StopBlinkingCursor().DecDECRQM(12));
            Assert.Equal("".CSI().Query().T("12;2$y"), toSend);
        }

        // !Cursor shape
        // PUSH "\e[3 q"
        //   settermprop 2 true
        //   settermprop 7 2
        [Fact]
        public void CursorShape()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "\e[3 q"
            //   settermprop 2 true
            //   settermprop 7 2
            Push(d, "".DECSCUSR(3));
            Assert.True(t.CursorState.BlinkingCursor);
            Assert.Equal(VtNetCore.VirtualTerminal.Enums.ECursorShape.Underline, t.CursorState.CursorShape);
        }

        // !Title
        // PUSH "\e]2;Here is my title\a"
        //   settermprop 4 "Here is my title"
        [Fact]
        public void Title()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            string windowTitle = "";
            t.WindowTitleChanged += (sender, args) =>
            {
                windowTitle = args.Text;
            };

            // !Title
            // PUSH "\e]2;\a"
            //   settermprop 4 ""
            Push(d, "".ChangeWindowTitle(""));
            Assert.Equal("", windowTitle);

            // !Title
            // PUSH "\e]2;Here is my title\a"
            //   settermprop 4 "Here is my title"
            Push(d, "".ChangeWindowTitle("Here is my title"));
            Assert.Equal("Here is my title", windowTitle);
        }
    }
}
