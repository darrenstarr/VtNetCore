using System;
using System.Collections.Generic;
using System.Text;
using VtNetCore.VirtualTerminal;
using VtNetCore.XTermParser;
using Xunit;

namespace VtNetCoreUnitTests
{
    public class LibvtermMoveCursor
    {
        private void Push(DataConsumer d, string s)
        {
            d.Push(Encoding.UTF8.GetBytes(s));
        }

        //!Implicit
        //PUSH "ABC"
        //  ?cursor = 0,3
        [Fact]
        public void Implicit()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "ABC");

            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(3, t.ViewPort.CursorPosition.Column);
        }

        //!Backspace
        //PUSH "\b"
        //  ?cursor = 0,2
        [Fact]
        public void Backspace()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "ABC\b");

            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(2, t.ViewPort.CursorPosition.Column);
        }

        //!Horizontal Tab
        //PUSH "\t"
        //  ?cursor = 0,8
        [Fact]
        public void HorizontalTab()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "ABC\b\t");

            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(8, t.ViewPort.CursorPosition.Column);
        }

        //!Carriage Return
        //PUSH "\r"
        //  ?cursor = 0,0
        [Fact]
        public void CarriageReturn()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "ABC\b\t\r");

            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(0, t.ViewPort.CursorPosition.Column);
        }

        //!Linefeed
        //PUSH "\n"
        //  ?cursor = 1,0
        [Fact]
        public void Linefeed()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "ABC\b\t\r\n");

            Assert.Equal(1, t.ViewPort.CursorPosition.Row);
            Assert.Equal(0, t.ViewPort.CursorPosition.Column);
        }

        //!Backspace bounded by lefthand edge
        //PUSH "\e[4;2H"
        //  ?cursor = 3,1
        //PUSH "\b"
        //  ?cursor = 3,0
        //PUSH "\b"
        //  ?cursor = 3,0
        [Fact]
        public void BackspaceBoundByLeftHandEdge()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "".CUP(4,2));

            Assert.Equal(3, t.ViewPort.CursorPosition.Row);
            Assert.Equal(1, t.ViewPort.CursorPosition.Column);

            Push(d, "\b");

            Assert.Equal(3, t.ViewPort.CursorPosition.Row);
            Assert.Equal(0, t.ViewPort.CursorPosition.Column);

            Push(d, "\b");

            Assert.Equal(3, t.ViewPort.CursorPosition.Row);
            Assert.Equal(0, t.ViewPort.CursorPosition.Column);
        }

        //!Backspace cancels phantom
        //PUSH "\e[4;80H"
        //  ?cursor = 3,79
        //PUSH "X"
        //  ?cursor = 3,79
        //PUSH "\b"
        //  ?cursor = 3,78
        [Fact]
        public void BackspaceCancelsPhantom()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "".CUP(4, 80));

            Assert.Equal(3, t.ViewPort.CursorPosition.Row);
            Assert.Equal(79, t.ViewPort.CursorPosition.Column);

            Push(d, "X");

            Assert.Equal(3, t.ViewPort.CursorPosition.Row);
            Assert.Equal(79, t.ViewPort.CursorPosition.Column);

            Push(d, "\b");

            Assert.Equal(3, t.ViewPort.CursorPosition.Row);
            Assert.Equal(78, t.ViewPort.CursorPosition.Column);
        }

        //!HT bounded by righthand edge
        //PUSH "\e[1;78H"
        //  ?cursor = 0,77
        //PUSH "\t"
        //  ?cursor = 0,79
        //PUSH "\t"
        //  ?cursor = 0,79
        [Fact]
        public void HorizontalTabBoundByRighthandEdge()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "".CUP(1, 78));

            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(77, t.ViewPort.CursorPosition.Column);

            Push(d, "\t");

            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(79, t.ViewPort.CursorPosition.Column);

            Push(d, "\t");

            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(79, t.ViewPort.CursorPosition.Column);
        }

        //!Index
        //PUSH "ABC\eD"
        //  ?cursor = 1,3
        [Fact]
        public void Index()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "ABC".IND());

            Assert.Equal(1, t.ViewPort.CursorPosition.Row);
            Assert.Equal(3, t.ViewPort.CursorPosition.Column);
        }

        //!Reverse Index
        //PUSH "\eM"
        //  ?cursor = 0,3
        [Fact]
        public void ReverseIndex()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "ABC".IND().RI());

            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(3, t.ViewPort.CursorPosition.Column);
        }

        //!Newline
        //PUSH "\eE"
        //  ?cursor = 1,0
        [Fact]
        public void Newline()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "ABC".IND().RI().NEL());

            Assert.Equal(1, t.ViewPort.CursorPosition.Row);
            Assert.Equal(0, t.ViewPort.CursorPosition.Column);
        }

        //!Cursor Forward (Should be cursor down)
        //PUSH "\e[B"
        //  ?cursor = 1,0
        //PUSH "\e[3B"
        //  ?cursor = 4,0
        //PUSH "\e[0B"
        //  ?cursor = 5,0
        [Fact]
        public void CursorForward()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "".CUD());
            Assert.Equal(1, t.ViewPort.CursorPosition.Row);
            Assert.Equal(0, t.ViewPort.CursorPosition.Column);

            Push(d, "".CUD(3));
            Assert.Equal(4, t.ViewPort.CursorPosition.Row);
            Assert.Equal(0, t.ViewPort.CursorPosition.Column);

            Push(d, "".CUD(0));
            Assert.Equal(5, t.ViewPort.CursorPosition.Row);
            Assert.Equal(0, t.ViewPort.CursorPosition.Column);
        }

        //!Cursor Down (Should be cursor forward)
        //PUSH "\e[C"
        //  ?cursor = 5,1
        //PUSH "\e[3C"
        //  ?cursor = 5,4
        //PUSH "\e[0C"
        //  ?cursor = 5,5
        [Fact]
        public void CursorDown()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "".CUD().CUD(3).CUD(0));

            Push(d, "".CUF());
            Assert.Equal(5, t.ViewPort.CursorPosition.Row);
            Assert.Equal(1, t.ViewPort.CursorPosition.Column);

            Push(d, "".CUF(3));
            Assert.Equal(5, t.ViewPort.CursorPosition.Row);
            Assert.Equal(4, t.ViewPort.CursorPosition.Column);

            Push(d, "".CUF(0));
            Assert.Equal(5, t.ViewPort.CursorPosition.Row);
            Assert.Equal(5, t.ViewPort.CursorPosition.Column);
        }

        //!Cursor Up
        //PUSH "\e[A"
        //  ?cursor = 4,5
        //PUSH "\e[3A"
        //  ?cursor = 1,5
        //PUSH "\e[0A"
        //  ?cursor = 0,5
        [Fact]
        public void CursorUp()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "".CUD().CUD(3).CUD(0).CUF().CUF(3).CUF(0));

            Push(d, "".CUU());
            Assert.Equal(4, t.ViewPort.CursorPosition.Row);
            Assert.Equal(5, t.ViewPort.CursorPosition.Column);

            Push(d, "".CUU(3));
            Assert.Equal(1, t.ViewPort.CursorPosition.Row);
            Assert.Equal(5, t.ViewPort.CursorPosition.Column);

            Push(d, "".CUU(0));
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(5, t.ViewPort.CursorPosition.Column);
        }

        //!Cursor Backward
        //PUSH "\e[D"
        //  ?cursor = 0,4
        //PUSH "\e[3D"
        //  ?cursor = 0,1
        //PUSH "\e[0D"
        //  ?cursor = 0,0
        [Fact]
        public void CursorBackward()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "".CUD().CUD(3).CUD(0).CUF().CUF(3).CUF(0).CUU().CUU(3).CUU(0));

            Push(d, "".CUB());
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(4, t.ViewPort.CursorPosition.Column);

            Push(d, "".CUB(3));
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(1, t.ViewPort.CursorPosition.Column);

            Push(d, "".CUB(0));
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(0, t.ViewPort.CursorPosition.Column);
        }

        //!Cursor Next Line
        //PUSH "   "
        //  ?cursor = 0,3
        //PUSH "\e[E"
        //  ?cursor = 1,0
        //PUSH "   "
        //  ?cursor = 1,3
        //PUSH "\e[2E"
        //  ?cursor = 3,0
        //PUSH "\e[0E"
        //  ?cursor = 4,0
        [Fact]
        public void CursorNextLine()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "   ");
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(3, t.ViewPort.CursorPosition.Column);

            Push(d, "".CNL());
            Assert.Equal(1, t.ViewPort.CursorPosition.Row);
            Assert.Equal(0, t.ViewPort.CursorPosition.Column);

            Push(d, "   ");
            Assert.Equal(1, t.ViewPort.CursorPosition.Row);
            Assert.Equal(3, t.ViewPort.CursorPosition.Column);

            Push(d, "".CNL(2));
            Assert.Equal(3, t.ViewPort.CursorPosition.Row);
            Assert.Equal(0, t.ViewPort.CursorPosition.Column);

            Push(d, "".CNL());
            Assert.Equal(4, t.ViewPort.CursorPosition.Row);
            Assert.Equal(0, t.ViewPort.CursorPosition.Column);
        }

        //!Cursor Previous Line
        //PUSH "   "
        //  ?cursor = 4,3
        //PUSH "\e[F"
        //  ?cursor = 3,0
        //PUSH "   "
        //  ?cursor = 3,3
        //PUSH "\e[2F"
        //  ?cursor = 1,0
        //PUSH "\e[0F"
        //  ?cursor = 0,0
        [Fact]
        public void CursorPreviousLine()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "   ".CNL().T("   ").CNL(2).CNL());

            Push(d, "   ");
            Assert.Equal(4, t.ViewPort.CursorPosition.Row);
            Assert.Equal(3, t.ViewPort.CursorPosition.Column);

            Push(d, "".CPL());
            Assert.Equal(3, t.ViewPort.CursorPosition.Row);
            Assert.Equal(0, t.ViewPort.CursorPosition.Column);

            Push(d, "   ");
            Assert.Equal(3, t.ViewPort.CursorPosition.Row);
            Assert.Equal(3, t.ViewPort.CursorPosition.Column);

            Push(d, "".CPL(2));
            Assert.Equal(1, t.ViewPort.CursorPosition.Row);
            Assert.Equal(0, t.ViewPort.CursorPosition.Column);

            Push(d, "".CPL());
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(0, t.ViewPort.CursorPosition.Column);
        }

        //!Cursor Horizonal Absolute
        //PUSH "\n"
        //  ?cursor = 1,0
        //PUSH "\e[20G"
        //  ?cursor = 1,19
        //PUSH "\e[G"
        //  ?cursor = 1,0
        [Fact]
        public void CursorHorizontalAbsolute()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "".LF());
            Assert.Equal(1, t.ViewPort.CursorPosition.Row);
            Assert.Equal(0, t.ViewPort.CursorPosition.Column);

            Push(d, "".CHA(20));
            Assert.Equal(1, t.ViewPort.CursorPosition.Row);
            Assert.Equal(19, t.ViewPort.CursorPosition.Column);

            Push(d, "".CHA());
            Assert.Equal(1, t.ViewPort.CursorPosition.Row);
            Assert.Equal(0, t.ViewPort.CursorPosition.Column);
        }

        //!Cursor Position
        //PUSH "\e[10;5H"
        //  ?cursor = 9,4
        //PUSH "\e[8H"
        //  ?cursor = 7,0
        //PUSH "\e[H"
        //  ?cursor = 0,0
        [Fact]
        public void CursorPosition()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "".CUP(10, 5));
            Assert.Equal(9, t.ViewPort.CursorPosition.Row);
            Assert.Equal(4, t.ViewPort.CursorPosition.Column);

            Push(d, "".CUP(8));
            Assert.Equal(7, t.ViewPort.CursorPosition.Row);
            Assert.Equal(0, t.ViewPort.CursorPosition.Column);

            Push(d, "".CUP());
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(0, t.ViewPort.CursorPosition.Column);
        }

        //!Cursor Position cancels phantom
        //PUSH "\e[10;78H"
        //  ?cursor = 9,77
        //PUSH "ABC"
        //  ?cursor = 9,79
        //PUSH "\e[10;80H"
        //PUSH "C"
        //  ?cursor = 9,79
        //PUSH "X"
        //  ?cursor = 10,1
        [Fact]
        public void CursorPositionCancelsPhantom()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "".CUP(10, 78));
            Assert.Equal(9, t.ViewPort.CursorPosition.Row);
            Assert.Equal(77, t.ViewPort.CursorPosition.Column);

            Push(d, "ABC");
            Assert.Equal(9, t.ViewPort.CursorPosition.Row);
            Assert.Equal(79, t.ViewPort.CursorPosition.Column);

            Push(d, "".CUP(10, 80).T("C"));
            Assert.Equal(9, t.ViewPort.CursorPosition.Row);
            Assert.Equal(79, t.ViewPort.CursorPosition.Column);

            Push(d, "X");
            Assert.Equal(10, t.ViewPort.CursorPosition.Row);
            Assert.Equal(1, t.ViewPort.CursorPosition.Column);
        }

        //!Bounds Checking
        //PUSH "\e[A"
        //  ?cursor = 0,0
        //PUSH "\e[D"
        //  ?cursor = 0,0
        //PUSH "\e[25;80H"
        //  ?cursor = 24,79
        //PUSH "\e[B"
        //  ?cursor = 24,79
        //PUSH "\e[C"
        //  ?cursor = 24,79
        //PUSH "\e[E"
        //  ?cursor = 24,0
        //PUSH "\e[H"
        //  ?cursor = 0,0
        //PUSH "\e[F"
        //  ?cursor = 0,0
        //PUSH "\e[999G"
        //  ?cursor = 0,79
        //PUSH "\e[99;99H"
        //  ?cursor = 24,79
        [Fact]
        public void BoundsChecking()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "".CUU());
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(0, t.ViewPort.CursorPosition.Column);

            Push(d, "".CUB());
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(0, t.ViewPort.CursorPosition.Column);

            Push(d, "".CUP(25, 80));
            Assert.Equal(24, t.ViewPort.CursorPosition.Row);
            Assert.Equal(79, t.ViewPort.CursorPosition.Column);

            Push(d, "".CUD());
            Assert.Equal(24, t.ViewPort.CursorPosition.Row);
            Assert.Equal(79, t.ViewPort.CursorPosition.Column);

            Push(d, "".CUF());
            Assert.Equal(24, t.ViewPort.CursorPosition.Row);
            Assert.Equal(79, t.ViewPort.CursorPosition.Column);

            Push(d, "".CHA(999));
            Assert.Equal(24, t.ViewPort.CursorPosition.Row);
            Assert.Equal(79, t.ViewPort.CursorPosition.Column);

            Push(d, "".CUP(99, 99));
            Assert.Equal(24, t.ViewPort.CursorPosition.Row);
            Assert.Equal(79, t.ViewPort.CursorPosition.Column);
        }

        //!Horizontal Position Absolute
        //PUSH "\e[5`"
        //  ?cursor = 0,4
        [Fact]
        public void HorizontalPositionAbsolute()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "".HPA(5));
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(4, t.ViewPort.CursorPosition.Column);
        }

        //!Horizontal Position Relative
        //PUSH "\e[3a"
        //  ?cursor = 0,7
        [Fact]
        public void HorizontalPositionRelative()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "".HPA(5).HPR(3));
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(7, t.ViewPort.CursorPosition.Column);
        }

        //!Horizontal Position Backward
        //PUSH "\e[3j"
        //  ?cursor = 0,4
        [Fact]
        public void HorizontalPositionBackward()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "".HPA(5).HPR(3).HPB(3));
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(4, t.ViewPort.CursorPosition.Column);
        }

        //!Horizontal and Vertical Position
        //PUSH "\e[3;3f"
        //  ?cursor = 2,2
        [Fact]
        public void HorizontalAndVerticalPosition()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "".HVP(3,3));
            Assert.Equal(2, t.ViewPort.CursorPosition.Row);
            Assert.Equal(2, t.ViewPort.CursorPosition.Column);
        }

        //!Vertical Position Absolute
        //PUSH "\e[5d"
        //  ?cursor = 4,2
        [Fact]
        public void VerticalPositionAbsolute()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "".HVP(3, 3).VPA(5));
            Assert.Equal(4, t.ViewPort.CursorPosition.Row);
            Assert.Equal(2, t.ViewPort.CursorPosition.Column);
        }

        //!Vertical Position Relative
        //PUSH "\e[2e"
        //  ?cursor = 6,2
        [Fact]
        public void VerticalPositionRelative()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "".HVP(3, 3).VPA(5).VPR(2));
            Assert.Equal(6, t.ViewPort.CursorPosition.Row);
            Assert.Equal(2, t.ViewPort.CursorPosition.Column);
        }

        //!Vertical Position Backward
        //PUSH "\e[2k"
        //  ?cursor = 4,2
        [Fact]
        public void VerticalPositionBackward()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "".HVP(3, 3).VPA(5).VPR(2).VPB(2));
            Assert.Equal(4, t.ViewPort.CursorPosition.Row);
            Assert.Equal(2, t.ViewPort.CursorPosition.Column);
        }

        //!Horizontal Tab
        //PUSH "\t"
        //  ?cursor = 0,8
        //PUSH "   "
        //  ?cursor = 0,11
        //PUSH "\t"
        //  ?cursor = 0,16
        //PUSH "       "
        //  ?cursor = 0,23
        //PUSH "\t"
        //  ?cursor = 0,24
        //PUSH "        "
        //  ?cursor = 0,32
        //PUSH "\t"
        //  ?cursor = 0,40
        [Fact]
        public void HorizontalTabs()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "\t");
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(8, t.ViewPort.CursorPosition.Column);

            Push(d, "   ");
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(11, t.ViewPort.CursorPosition.Column);

            Push(d, "\t");
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(16, t.ViewPort.CursorPosition.Column);

            Push(d, "       ");
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(23, t.ViewPort.CursorPosition.Column);

            Push(d, "\t");
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(24, t.ViewPort.CursorPosition.Column);

            Push(d, "        ");
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(32, t.ViewPort.CursorPosition.Column);

            Push(d, "\t");
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(40, t.ViewPort.CursorPosition.Column);
        }

        //!Cursor Horizontal Tab
        //PUSH "\e[I"
        //  ?cursor = 0,48
        //PUSH "\e[2I"
        //  ?cursor = 0,64

        [Fact]
        public void CursorHorizontalTabs()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "\t   \t       \t        \t".CHT());
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(48, t.ViewPort.CursorPosition.Column);

            Push(d, "".CHT(2));
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(64, t.ViewPort.CursorPosition.Column);
        }

        //!Cursor Backward Tab
        //PUSH "\e[Z"
        //  ?cursor = 0,56
        //PUSH "\e[2Z"
        //  ?cursor = 0,40
        [Fact]
        public void CursorBackwardTabs()
        {
            var t = new VirtualTerminalController();
            t.ResizeView(80, 25);

            var d = new DataConsumer(t);

            Push(d, "\t   \t       \t        \t".CHT().CHT(2).CBT());
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(56, t.ViewPort.CursorPosition.Column);

            Push(d, "".CBT(2));
            Assert.Equal(0, t.ViewPort.CursorPosition.Row);
            Assert.Equal(40, t.ViewPort.CursorPosition.Column);
        }
    }
}
