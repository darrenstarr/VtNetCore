namespace VtNetCoreUnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using VtNetCore.VirtualTerminal;
    using VtNetCore.XTermParser;
    using Xunit;

    public class ScrollBug
    {
        private void Push(DataConsumer d, string s)
        {
            d.Push(Encoding.UTF8.GetBytes(s));
        }

        private bool IsCursor(VirtualTerminalController t, int row, int column)
        {
            return t.ViewPort.CursorPosition.Row == row && t.ViewPort.CursorPosition.Column == column;
        }

        [Fact]
        public void MoveCursorPastEndOfScreen()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(41, 18);
            t.TestPatternScrollingDiagonalLower();

            // \e[?1l
            Push(d, "".DECRST(1));  // Ps = 1  -> Normal Cursor Keys (DECCKM).
            // \e>
            Push(d, "".DECKPNM());  // Normal keypad
            // \e[19;1H
            Push(d, "".CUP(19, 1));

            Assert.True(IsCursor(t, 17, 0));
        }
    }
}
