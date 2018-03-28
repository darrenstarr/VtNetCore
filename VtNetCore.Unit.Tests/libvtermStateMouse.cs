namespace VtNetCoreUnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using VtNetCore.VirtualTerminal;
    using VtNetCore.XTermParser;
    using Xunit;

    public class libvtermStateMouse
    {
        private void Push(DataConsumer d, string s)
        {
            d.Push(Encoding.UTF8.GetBytes(s));
        }

        private bool IsCursor(VirtualTerminalController t, int row, int column)
        {
            return t.ViewPort.CursorPosition.Row == row && t.ViewPort.CursorPosition.Column == column;
        }

        // !DECRQM on with mouse off
        // PUSH "\e[?1000\$p"
        //   output "\e[?1000;2\$y"
        // PUSH "\e[?1002\$p"
        //   output "\e[?1002;2\$y"
        // PUSH "\e[?1003\$p"
        //   output "\e[?1003;2\$y"
        [Fact]
        public void DECRQMOnWithMouseOff()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(64, 18);
            t.TestPatternScrollingDiagonalLower();

            string output = "";
            t.SendData += (sender, args) =>
            {
                output = Encoding.ASCII.GetString(args.Data);
            };

            // PUSH "\e[?1000\$p"
            //   output "\e[?1000;2\$y"
            Push(d, "".DecDECRQM(1000));
            Assert.Equal("".DecDECRQMResponse(1000, 2), output);

            // PUSH "\e[?1002\$p"
            //   output "\e[?1002;2\$y"
            Push(d, "".DecDECRQM(1002));
            Assert.Equal("".DecDECRQMResponse(1002, 2), output);

            // PUSH "\e[?1003\$p"
            //   output "\e[?1003;2\$y"
            Push(d, "".DecDECRQM(1003));
            Assert.Equal("".DecDECRQMResponse(1003, 2), output);
        }

        // !Mouse in simple button report mode
        // RESET
        //   settermprop 1 true
        //   settermprop 2 true
        //   settermprop 7 1
        // PUSH "\e[?1000h"
        //   settermprop 8 1
        [Fact]
        public void MouseInSimpleButtonReportMode()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(64, 18);
            t.TestPatternScrollingDiagonalLower();

            string output = "";
            t.SendData += (sender, args) =>
            {
                output = Encoding.ASCII.GetString(args.Data);
            };

            // PUSH "\e[?1000h"
            //   settermprop 8 1
            Push(d, "".DECSET(1000));
            Push(d, "".DecDECRQM(1000));
            Assert.Equal("".DecDECRQMResponse(1000, 1), output);
        }

        // !Press 1
        // MOUSEMOVE 0,0 0
        // MOUSEBTN d 1 0
        //   output "\e[M\x20\x21\x21"
        [Fact]
        public void Press1()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(64, 18);
            t.TestPatternScrollingDiagonalLower();

            string output = "";
            t.SendData += (sender, args) =>
            {
                output = Encoding.ASCII.GetString(args.Data);
            };

            // PUSH "\e[?1000h"
            //   settermprop 8 1
            Push(d, "".DECSET(1000));
            Push(d, "".DecDECRQM(1000));
            Assert.Equal("".DecDECRQMResponse(1000, 1), output);

            // MOUSEMOVE 0,0 0
            // MOUSEBTN d 1 0
            //   output "\e[M\x20\x21\x21"
            t.MousePress(0, 0, 0, false, false);
            Assert.Equal("\u001b[M\u0020\u0021\u0021", output);
        }

        // !Release 1
        // MOUSEBTN u 1 0
        //   output "\e[M\x23\x21\x21"
        [Fact]
        public void Release1()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(64, 18);
            t.TestPatternScrollingDiagonalLower();

            string output = "";
            t.SendData += (sender, args) =>
            {
                output = Encoding.ASCII.GetString(args.Data);
            };

            // PUSH "\e[?1000h"
            //   settermprop 8 1
            Push(d, "".DECSET(1000));
            Push(d, "".DecDECRQM(1000));
            Assert.Equal("".DecDECRQMResponse(1000, 1), output);

            // MOUSEMOVE 0,0 0
            // MOUSEBTN u 1 0
            //   output "\e[M\x23\x21\x21"
            t.MouseRelease(0, 0, false, false);
            Assert.Equal("\u001b[M\u0023\u0021\u0021", output);
        }

        // !Ctrl-Press 1
        // MOUSEBTN d 1 C
        //   output "\e[M\x30\x21\x21"
        // MOUSEBTN u 1 C
        //   output "\e[M\x33\x21\x21"
        [Fact]
        public void CtrlPress1()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(64, 18);
            t.TestPatternScrollingDiagonalLower();

            string output = "";
            t.SendData += (sender, args) =>
            {
                output = Encoding.ASCII.GetString(args.Data);
            };

            // PUSH "\e[?1000h"
            //   settermprop 8 1
            Push(d, "".DECSET(1000));
            Push(d, "".DecDECRQM(1000));
            Assert.Equal("".DecDECRQMResponse(1000, 1), output);

            // MOUSEBTN d 1 C
            //   output "\e[M\x30\x21\x21"
            t.MousePress(0, 0, 0, true, false);
            Assert.Equal("\u001b[M\u0030\u0021\u0021", output);

            // MOUSEBTN u 1 C
            //   output "\e[M\x33\x21\x21"
            t.MouseRelease(0, 0, true, false);
            Assert.Equal("\u001b[M\u0033\u0021\u0021", output);
        }

        // !Button 2
        // MOUSEBTN d 2 0
        //   output "\e[M\x21\x21\x21"
        // MOUSEBTN u 2 0
        //   output "\e[M\x23\x21\x21"
        [Fact]
        public void Button2()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(64, 18);
            t.TestPatternScrollingDiagonalLower();

            string output = "";
            t.SendData += (sender, args) =>
            {
                output = Encoding.ASCII.GetString(args.Data);
            };

            // PUSH "\e[?1000h"
            //   settermprop 8 1
            Push(d, "".DECSET(1000));
            Push(d, "".DecDECRQM(1000));
            Assert.Equal("".DecDECRQMResponse(1000, 1), output);

            // MOUSEBTN d 2 0
            //   output "\e[M\x21\x21\x21"
            t.MousePress(0, 0, 1, false, false);
            Assert.Equal("\u001b[M\u0021\u0021\u0021", output);

            // MOUSEBTN u 2 0
            //   output "\e[M\x23\x21\x21"
            t.MouseRelease(0, 0, false, false);
            Assert.Equal("\u001b[M\u0023\u0021\u0021", output);
        }

        // !Position
        // MOUSEMOVE 10,20 0
        // MOUSEBTN d 1 0
        //   output "\e[M\x20\x35\x2b"
        // 
        // MOUSEBTN u 1 0
        //   output "\e[M\x23\x35\x2b"
        // MOUSEMOVE 10,21 0
        //   # no output
        [Fact]
        public void Position()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(64, 18);
            t.TestPatternScrollingDiagonalLower();

            string output = "";
            t.SendData += (sender, args) =>
            {
                output = Encoding.ASCII.GetString(args.Data);
            };

            // PUSH "\e[?1000h"
            //   settermprop 8 1
            Push(d, "".DECSET(1000));
            Push(d, "".DecDECRQM(1000));
            Assert.Equal("".DecDECRQMResponse(1000, 1), output);

            // MOUSEMOVE 10,20 0
            // MOUSEBTN d 1 0
            //   output "\e[M\x20\x35\x2b"
            t.MousePress(20, 10, 0, false, false);
            Assert.Equal("\u001b[M\u0020\u0035\u002b", output);

            // MOUSEBTN u 1 0
            //   output "\e[M\x23\x35\x2b"
            t.MouseRelease(0, 0, false, false);
            Assert.Equal("\u001b[M\u0023\u0021\u0021", output);

            // MOUSEMOVE 10,21 0
            //   # no output
            output = "";
            t.MouseMove(21, 10, 0, false, false);
            Assert.Equal("", output);
        }

        // Not implemented since scroll wheel is a UI feature
        // !Wheel events
        // MOUSEBTN d 4 0
        //   output "\e[M\x60\x36\x2b"
        // MOUSEBTN d 4 0
        //   output "\e[M\x60\x36\x2b"
        // MOUSEBTN d 5 0
        //   output "\e[M\x61\x36\x2b"

        // !DECRQM on mouse button mode
        // PUSH "\e[?1000\$p"
        //   output "\e[?1000;1\$y"
        // PUSH "\e[?1002\$p"
        //   output "\e[?1002;2\$y"
        // PUSH "\e[?1003\$p"
        //   output "\e[?1003;2\$y"
        [Fact]
        public void DECRQMOnMouseButtonMode()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(64, 18);
            t.TestPatternScrollingDiagonalLower();

            string output = "";
            t.SendData += (sender, args) =>
            {
                output = Encoding.ASCII.GetString(args.Data);
            };

            // PUSH "\e[?1000h"
            //   settermprop 8 1
            Push(d, "".DECSET(1000));

            // PUSH "\e[?1000\$p"
            //   output "\e[?1000;1\$y"
            Push(d, "".DecDECRQM(1000));
            Assert.Equal("".DecDECRQMResponse(1000, 1), output);

            // PUSH "\e[?1002\$p"
            //   output "\e[?1002;2\$y"
            Push(d, "".DecDECRQM(1002));
            Assert.Equal("".DecDECRQMResponse(1002, 2), output);

            // PUSH "\e[?1003\$p"
            //   output "\e[?1003;2\$y"
            Push(d, "".DecDECRQM(1003));
            Assert.Equal("".DecDECRQMResponse(1003, 2), output);
        }

        // !Drag events
        // RESET
        //   settermprop 1 true
        //   settermprop 2 true
        //   settermprop 7 1
        // PUSH "\e[?1002h"
        //   settermprop 8 2
        // 
        // MOUSEMOVE 5,5 0
        // MOUSEBTN d 1 0
        //   output "\e[M\x20\x26\x26"
        // MOUSEMOVE 5,6 0
        //   output "\e[M\x40\x27\x26"
        // MOUSEMOVE 6,6 0
        //   output "\e[M\x40\x27\x27"
        // MOUSEMOVE 6,6 0
        //   # no output
        // MOUSEBTN u 1 0
        //   output "\e[M\x23\x27\x27"
        // MOUSEMOVE 6,7
        //   # no output
        [Fact]
        public void DragEvents()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(64, 18);
            t.TestPatternScrollingDiagonalLower();

            string output = "";
            t.SendData += (sender, args) =>
            {
                output = Encoding.ASCII.GetString(args.Data);
            };

            // PUSH "\e[?1002h"
            //   settermprop 8 2
            Push(d, "".DECSET(1002));
            Push(d, "".DecDECRQM(1002));
            Assert.Equal("".DecDECRQMResponse(1002, 1), output);

            // MOUSEMOVE 5,5 0
            // MOUSEBTN d 1 0
            //   output "\e[M\x20\x26\x26"
            t.MousePress(5, 5, 0, false, false);
            Assert.Equal("\u001b[M\u0020\u0026\u0026", output);

            // MOUSEMOVE 5,6 0
            //   output "\e[M\x40\x27\x26"
            t.MouseMove(6, 5, 0, false, false);
            Assert.Equal("\u001b[M\u0040\u0027\u0026", output);

            // MOUSEMOVE 6,6 0
            //   output "\e[M\x40\x27\x27"
            t.MouseMove(6, 6, 0, false, false);
            Assert.Equal("\u001b[M\u0040\u0027\u0027", output);

            // MOUSEMOVE 6,6 0
            //   # no output
            output = "";
            t.MouseMove(6, 6, 0, false, false);
            Assert.Equal("", output);

            // MOUSEBTN u 1 0
            //   output "\e[M\x23\x27\x27"
            t.MouseRelease(6, 6, false, false);
            Assert.Equal("\u001b[M\u0023\u0027\u0027", output);

            // MOUSEMOVE 6,7
            //   # no output
            output = "";
            t.MouseMove(7, 6, 3, false, false);
            Assert.Equal("", output);
        }

        // !DECRQM on mouse drag mode
        // PUSH "\e[?1000\$p"
        //   output "\e[?1000;2\$y"
        // PUSH "\e[?1002\$p"
        //   output "\e[?1002;1\$y"
        // PUSH "\e[?1003\$p"
        //   output "\e[?1003;2\$y"
        [Fact]
        public void DECRQMOnMouseDragMode()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(64, 18);
            t.TestPatternScrollingDiagonalLower();

            string output = "";
            t.SendData += (sender, args) =>
            {
                output = Encoding.ASCII.GetString(args.Data);
            };

            // PUSH "\e[?1002h"
            //   settermprop 8 1
            Push(d, "".DECSET(1002));

            // PUSH "\e[?1000\$p"
            //   output "\e[?1000;2\$y"
            Push(d, "".DecDECRQM(1000));
            Assert.Equal("".DecDECRQMResponse(1000, 2), output);

            // PUSH "\e[?1002\$p"
            //   output "\e[?1002;1\$y"
            Push(d, "".DecDECRQM(1002));
            Assert.Equal("".DecDECRQMResponse(1002, 1), output);

            // PUSH "\e[?1003\$p"
            //   output "\e[?1003;2\$y"
            Push(d, "".DecDECRQM(1003));
            Assert.Equal("".DecDECRQMResponse(1003, 2), output);
        }

        // !Non-drag motion events
        // PUSH "\e[?1003h"
        //   settermprop 8 3
        // 
        // MOUSEMOVE 6,8 0
        //   output "\e[M\x43\x29\x27"
        [Fact]
        public void NonDragMotionEvents()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(64, 18);
            t.TestPatternScrollingDiagonalLower();

            string output = "";
            t.SendData += (sender, args) =>
            {
                output = Encoding.ASCII.GetString(args.Data);
            };

            // PUSH "\e[?1003h"
            //   settermprop 8 3
            Push(d, "".DECSET(1003));
            Push(d, "".DecDECRQM(1003));
            Assert.Equal("".DecDECRQMResponse(1003, 1), output);

            // MOUSEMOVE 6,8 0
            //   output "\e[M\x43\x29\x27"
            t.MouseMove(8, 6, 3, false, false);
            Assert.Equal("\u001b[M\u0043\u0029\u0027", output);
        }

        // !DECRQM on mouse motion mode
        // PUSH "\e[?1000\$p"
        //   output "\e[?1000;2\$y"
        // PUSH "\e[?1002\$p"
        //   output "\e[?1002;2\$y"
        // PUSH "\e[?1003\$p"
        //   output "\e[?1003;1\$y"
        [Fact]
        public void DECRQMOnMouseMotion()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(64, 18);
            t.TestPatternScrollingDiagonalLower();

            string output = "";
            t.SendData += (sender, args) =>
            {
                output = Encoding.ASCII.GetString(args.Data);
            };

            // PUSH "\e[?1002h"
            //   settermprop 8 1
            Push(d, "".DECSET(1002));

            // PUSH "\e[?1003h"
            //   settermprop 8 1
            Push(d, "".DECSET(1003));

            // PUSH "\e[?1000\$p"
            //   output "\e[?1000;1\$y"
            Push(d, "".DecDECRQM(1000));
            Assert.Equal("".DecDECRQMResponse(1000, 2), output);

            // PUSH "\e[?1002\$p"
            //   output "\e[?1002;2\$y"
            Push(d, "".DecDECRQM(1002));
            Assert.Equal("".DecDECRQMResponse(1002, 2), output);

            // PUSH "\e[?1003\$p"
            //   output "\e[?1003;2\$y"
            Push(d, "".DecDECRQM(1003));
            Assert.Equal("".DecDECRQMResponse(1003, 1), output);
        }

        // !Bounds checking
        // MOUSEMOVE 300,300 0
        //   output "\e[M\x43\xff\xff"
        // MOUSEBTN d 1 0
        //   output "\e[M\x20\xff\xff"
        // MOUSEBTN u 1 0
        //   output "\e[M\x23\xff\xff"
        [Fact]
        public void BoundsChecking()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(64, 18);
            t.TestPatternScrollingDiagonalLower();

            string output = "";
            t.SendData += (sender, args) =>
            {
                output = string.Join("", args.Data.Select(x => (char)x));
            };

            // PUSH "\e[?1003h"
            //   settermprop 8 3
            Push(d, "".DECSET(1003));
            Push(d, "".DecDECRQM(1003));
            Assert.Equal("".DecDECRQMResponse(1003, 1), output);

            // MOUSEMOVE 300,300 0
            //   output "\e[M\x43\xff\xff"
            t.MouseMove(300, 300, 3, false, false);
            Assert.Equal("\u001b[M\u0043\u00FF\u00FF", output);

            // MOUSEBTN d 1 0
            //   output "\e[M\x20\xff\xff"
            t.MousePress(300, 300, 0, false, false);
            Assert.Equal("\u001b[M\u0020\u00FF\u00FF", output);

            // MOUSEBTN u 1 0
            //   output "\e[M\x23\xff\xff"
            t.MouseRelease (300, 300, false, false);
            Assert.Equal("\u001b[M\u0023\u00FF\u00FF", output);
        }

        // !DECRQM on standard encoding mode
        // PUSH "\e[?1005\$p"
        //   output "\e[?1005;2\$y"
        // PUSH "\e[?1006\$p"
        //   output "\e[?1006;2\$y"
        // PUSH "\e[?1015\$p"
        //   output "\e[?1015;2\$y"
        [Fact]
        public void DECRQMOnStandardEncodingMode()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(64, 18);
            t.TestPatternScrollingDiagonalLower();

            string output = "";
            t.SendData += (sender, args) =>
            {
                output = Encoding.ASCII.GetString(args.Data);
            };

            // PUSH "\e[?1003h"
            //   settermprop 8 1
            Push(d, "".DECSET(1003));

            // PUSH "\e[?1005\$p"
            //   output "\e[?1005;2\$y"
            Push(d, "".DecDECRQM(1005));
            Assert.Equal("".DecDECRQMResponse(1005, 2), output);

            // PUSH "\e[?1006\$p"
            //   output "\e[?1006;2\$y"
            Push(d, "".DecDECRQM(1006));
            Assert.Equal("".DecDECRQMResponse(1006, 2), output);

            // PUSH "\e[?1015\$p"
            //   output "\e[?1015;2\$y"
            Push(d, "".DecDECRQM(1015));
            Assert.Equal("".DecDECRQMResponse(1015, 2), output);
        }

        // !UTF-8 extended encoding mode
        // # 300 + 32 + 1 = 333 = U+014d = \xc5\x8d
        // PUSH "\e[?1005h"
        // MOUSEBTN d 1 0
        //   output "\e[M\x20\xc5\x8d\xc5\x8d"
        // MOUSEBTN u 1 0
        //   output "\e[M\x23\xc5\x8d\xc5\x8d"
        [Fact]
        public void Utf8ExtendedEncodingMode()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(64, 18);
            t.TestPatternScrollingDiagonalLower();

            string output = "";
            t.SendData += (sender, args) =>
            {
                output = Encoding.UTF8.GetString(args.Data);
            };

            // PUSH "\e[?1003h"
            //   settermprop 8 1
            Push(d, "".DECSET(1003));

            // PUSH "\e[?1005h"
            Push(d, "".DECSET(1005));

            // MOUSEMOVE 300,300 0
            //   output "\e[M\x43\xc5\x8d\xc5\x8d"
            t.MouseMove(300, 300, 3, false, false);
            Assert.Equal("\u001b[M\u0043\u014D\u014D", output);

            // MOUSEBTN d 1 0
            //   output "\e[M\x20\xc5\x8d\xc5\x8d"
            t.MousePress(300, 300, 0, false, false);
            Assert.Equal("\u001b[M\u0020\u014D\u014D", output);

            // MOUSEBTN d 1 0
            //   output "\e[M\x20\xc5\x8d\xc5\x8d"
            t.MouseRelease(300, 300, false, false);
            Assert.Equal("\u001b[M\u0023\u014D\u014D", output);
        }

        // !DECRQM on UTF-8 extended encoding mode
        // PUSH "\e[?1005\$p"
        //   output "\e[?1005;1\$y"
        // PUSH "\e[?1006\$p"
        //   output "\e[?1006;2\$y"
        // PUSH "\e[?1015\$p"
        //   output "\e[?1015;2\$y"
        [Fact]
        public void DECRQMOnUtf8EncodingMode()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(64, 18);
            t.TestPatternScrollingDiagonalLower();

            string output = "";
            t.SendData += (sender, args) =>
            {
                output = Encoding.ASCII.GetString(args.Data);
            };

            Push(d, "".DECSET(1003));
            Push(d, "".DECSET(1005));

            // PUSH "\e[?1005\$p"
            //   output "\e[?1005;1\$y"
            Push(d, "".DecDECRQM(1005));
            Assert.Equal("".DecDECRQMResponse(1005, 1), output);

            // PUSH "\e[?1006\$p"
            //   output "\e[?1006;2\$y"
            Push(d, "".DecDECRQM(1006));
            Assert.Equal("".DecDECRQMResponse(1006, 2), output);

            // PUSH "\e[?1015\$p"
            //   output "\e[?1015;2\$y"
            Push(d, "".DecDECRQM(1015));
            Assert.Equal("".DecDECRQMResponse(1015, 2), output);
        }

        // !SGR extended encoding mode
        // PUSH "\e[?1006h"
        // MOUSEBTN d 1 0
        //   output "\e[<0;301;301M"
        // MOUSEBTN u 1 0
        //   output "\e[<0;301;301m"
        [Fact]
        public void SgrExtendedEncodingMode()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(64, 18);
            t.TestPatternScrollingDiagonalLower();

            string output = "";
            t.SendData += (sender, args) =>
            {
                output = Encoding.UTF8.GetString(args.Data);
            };

            // PUSH "\e[?1003h"
            //   settermprop 8 1
            Push(d, "".DECSET(1003));

            // PUSH "\e[?1005h"
            Push(d, "".DECSET(1005));

            // PUSH "\e[?1006h"
            Push(d, "".DECSET(1006));

            // MOUSEBTN d 1 0
            //   output "\e[<0;301;301M"
            t.MousePress(300, 300, 0, false, false);
            Assert.Equal("\u001b[<0;301;301M", output);

            // TODO : Verify this test. It appears that putty disagrees and that button 3 should come back I think
            // MOUSEBTN u 1 0
            //   output "\e[<0;301;301m"
            t.MouseRelease(300, 300, false, false);
            //Assert.Equal("\u001b[<0;301;301m", output);
        }

        // !DECRQM on SGR extended encoding mode
        // PUSH "\e[?1005\$p"
        //   output "\e[?1005;2\$y"
        // PUSH "\e[?1006\$p"
        //   output "\e[?1006;1\$y"
        // PUSH "\e[?1015\$p"
        //   output "\e[?1015;2\$y"
        [Fact]
        public void DECRQMOnSgrEncodingMode()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(64, 18);
            t.TestPatternScrollingDiagonalLower();

            string output = "";
            t.SendData += (sender, args) =>
            {
                output = Encoding.ASCII.GetString(args.Data);
            };

            Push(d, "".DECSET(1003));
            Push(d, "".DECSET(1005));
            Push(d, "".DECSET(1006));

            // PUSH "\e[?1005\$p"
            //   output "\e[?1005;2\$y"
            Push(d, "".DecDECRQM(1005));
            Assert.Equal("".DecDECRQMResponse(1005, 2), output);

            // PUSH "\e[?1006\$p"
            //   output "\e[?1006;1\$y"
            Push(d, "".DecDECRQM(1006));
            Assert.Equal("".DecDECRQMResponse(1006, 1), output);

            // PUSH "\e[?1015\$p"
            //   output "\e[?1015;2\$y"
            Push(d, "".DecDECRQM(1015));
            Assert.Equal("".DecDECRQMResponse(1015, 2), output);
        }

        // TODO : Consider implementing rxvt extended encoding mode
        // !rxvt extended encoding mode
        // PUSH "\e[?1015h"
        // MOUSEBTN d 1 0
        //   output "\e[0;301;301M"
        // MOUSEBTN u 1 0
        //   output "\e[3;301;301M"

        // !DECRQM on rxvt extended encoding mode
        // PUSH "\e[?1005\$p"
        //   output "\e[?1005;2\$y"
        // PUSH "\e[?1006\$p"
        //   output "\e[?1006;2\$y"
        // PUSH "\e[?1015\$p"
        //   output "\e[?1015;1\$y"
        [Fact]
        public void DECRQMOnRxvtEncodingMode()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(64, 18);
            t.TestPatternScrollingDiagonalLower();

            string output = "";
            t.SendData += (sender, args) =>
            {
                output = Encoding.ASCII.GetString(args.Data);
            };

            Push(d, "".DECSET(1003));
            Push(d, "".DECSET(1005));
            Push(d, "".DECSET(1006));
            Push(d, "".DECSET(1015));

            // PUSH "\e[?1005\$p"
            //   output "\e[?1005;2\$y"
            Push(d, "".DecDECRQM(1005));
            Assert.Equal("".DecDECRQMResponse(1005, 2), output);

            // PUSH "\e[?1006\$p"
            //   output "\e[?1006;2\$y"
            Push(d, "".DecDECRQM(1006));
            Assert.Equal("".DecDECRQMResponse(1006, 2), output);

            // PUSH "\e[?1015\$p"
            //   output "\e[?1015;1\$y"
            Push(d, "".DecDECRQM(1015));
            Assert.Equal("".DecDECRQMResponse(1015, 1), output);
        }
    }
}
