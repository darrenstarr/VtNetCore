using System;
using System.Collections.Generic;
using System.Text;
using VtNetCore.VirtualTerminal;
using VtNetCore.XTermParser;
using Xunit;

namespace VtNetCoreUnitTests
{
    public class BackgroundColorBug
    {
        private void Push(DataConsumer d, string s)
        {
            d.Push(Encoding.UTF8.GetBytes(s));
        }

        private bool IsCursor(VirtualTerminalController t, int row, int column)
        {
            return t.ViewPort.CursorPosition.Row == row && t.ViewPort.CursorPosition.Column == column;
        }

        private void FirstPage(VirtualTerminalController t, DataConsumer d)
        {
            Push(d, "\u001b[2J");
            Push(d, "\u001b[0;33;44m");
            Push(d, "\u001b[1;1H");
            Push(d, "\u001b[?7h");
            Push(d, "****************************************************************************************************************************************************************");
            Push(d, "\u001b[?7l");
            Push(d, "\u001b[3;1H");
            Push(d, "****************************************************************************************************************************************************************");
            Push(d, "\u001b[?7h");
            Push(d, "\u001b[5;1H");
            Push(d, "This should be three identical lines of *'s completely filling\r");
        }

        public static readonly string ExpectedFirstPage =
            "<foreground value='#CDCD00' /><background value='#0000CD' />********************************************************************************↵" +
            "********************************************************************************↵" +
            "********************************************************************************↵" +
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" +
            "This should be three identical lines of *'s completely filling→→→→→→→→→→→→→→→→→→↵" +
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" +
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" +
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" +
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" +
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" +
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" +
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" +
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" +
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" +
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" +
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" +
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" +
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" +
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" +
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" +
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" +
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" +
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" +
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→";

         [Fact]
        public void TestFirstPage()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 24);
            //t.TestPatternScrollingDiagonalLower();

            FirstPage(t, d);

            s = t.PageAsSpans;

            Assert.Equal(ExpectedFirstPage, s);
        }

        public static readonly string ExpectedDarkBackgroundPage =
            "<foreground value='#CDCD00' /><background value='#0000CD' />                   Graphic rendition test pattern:→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" + 
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" + 
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" + 
            "<foreground value='#CDCDCD' /><background value='#000000' />vanilla<foreground value='#CDCD00' /><background value='#0000CD' />                                <bright><foreground value='#FFFFFF' /><background value='#000000' />bold→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" + 
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" + 
            "</bright><foreground value='#CDCD00' /><background value='#0000CD' />     <underscore><foreground value='#CDCDCD' /><background value='#000000' />underline</underscore><foreground value='#CDCD00' /><background value='#0000CD' />                              <bright><underscore><foreground value='#FFFFFF' /><background value='#000000' />bold underline→→→→→→→→→→→→→→→→→→→→→→↵" + 
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" + 
            "<blink></bright></underscore><foreground value='#CDCDCD' />blink</blink><foreground value='#CDCD00' /><background value='#0000CD' />                                  <blink><bright><foreground value='#FFFFFF' /><background value='#000000' />bold blink→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" + 
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" + 
            "</blink></bright><foreground value='#CDCD00' /><background value='#0000CD' />     <blink><underscore><foreground value='#CDCDCD' /><background value='#000000' />underline blink</blink></underscore><foreground value='#CDCD00' /><background value='#0000CD' />                        <blink><bright><underscore><foreground value='#FFFFFF' /><background value='#000000' />bold underline blink→→→→→→→→→→→→→→→→↵" + 
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" + 
            "</blink></bright><reverse></underscore><foreground value='#CDCDCD' />negative</reverse><foreground value='#CDCD00' /><background value='#0000CD' />                               <bright><reverse><foreground value='#FFFFFF' /><background value='#000000' />bold negative→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" + 
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" + 
            "</bright></reverse><foreground value='#CDCD00' /><background value='#0000CD' />     <reverse><underscore><foreground value='#CDCDCD' /><background value='#000000' />underline negative</reverse></underscore><foreground value='#CDCD00' /><background value='#0000CD' />                     <bright><reverse><underscore><foreground value='#FFFFFF' /><background value='#000000' />bold underline negative→→→→→→→→→→→→→↵" + 
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" + 
            "<blink></bright></underscore><foreground value='#CDCDCD' />blink negative</blink></reverse><foreground value='#CDCD00' /><background value='#0000CD' />                         <blink><bright><reverse><foreground value='#FFFFFF' /><background value='#000000' />bold blink negative→→→→→→→→→→→→→→→→→→→→→→↵" + 
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" + 
            "</blink></bright></reverse><foreground value='#CDCD00' /><background value='#0000CD' />     <blink><reverse><underscore><foreground value='#CDCDCD' /><background value='#000000' />underline blink negative</blink></reverse></underscore><foreground value='#CDCD00' /><background value='#0000CD' />               <blink><bright><reverse><underscore><foreground value='#FFFFFF' /><background value='#000000' />bold underline blink negative→→→→→→→↵" + 
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" + 
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" + 
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" + 
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→↵" + 
            "</blink></bright></reverse></underscore><foreground value='#CDCDCD' />Dark background.Push<RETURN>                                                    ↵" + 
            "→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→→";

        private void DarkBackgroundPage(VirtualTerminalController t, DataConsumer d)
        {
            Push(d, "\u001b[1;24r");
            Push(d, "\u001b[2J");
            Push(d, "\u001b[1;20H");
            Push(d, "Graphic rendition test pattern:");
            Push(d, "\u001b[4;1H");
            Push(d, "\u001b[0m");
            Push(d, "vanilla");
            Push(d, "\u001b[4;40H");
            Push(d, "\u001b[0;1m");
            Push(d, "bold");
            Push(d, "\u001b[6;6H");
            Push(d, "\u001b[;4m");
            Push(d, "underline");
            Push(d, "\u001b[6;45H");
            Push(d, "\u001b[;1m");
            Push(d, "\u001b[4m");
            Push(d, "bold underline");
            Push(d, "\u001b[8;1H");
            Push(d, "\u001b[0;5m");
            Push(d, "blink");
            Push(d, "\u001b[8;40H");
            Push(d, "\u001b[0;5;1m");
            Push(d, "bold blink");
            Push(d, "\u001b[10;6H");
            Push(d, "\u001b[0;4;5m");
            Push(d, "underline blink");
            Push(d, "\u001b[10;45H");
            Push(d, "\u001b[0;1;4;5m");
            Push(d, "bold underline blink");
            Push(d, "\u001b[12;1H");
            Push(d, "\u001b[1;4;5;0;7m");
            Push(d, "negative");
            Push(d, "\u001b[12;40H");
            Push(d, "\u001b[0;1;7m");
            Push(d, "bold negative");
            Push(d, "\u001b[14;6H");
            Push(d, "\u001b[0;4;7m");
            Push(d, "underline negative");
            Push(d, "\u001b[14;45H");
            Push(d, "\u001b[0;1;4;7m");
            Push(d, "bold underline negative");
            Push(d, "\u001b[16;1H");
            Push(d, "\u001b[1;4;;5;7m");
            Push(d, "blink negative");
            Push(d, "\u001b[16;40H");
            Push(d, "\u001b[0;1;5;7m");
            Push(d, "bold blink negative");
            Push(d, "\u001b[18;6H");
            Push(d, "\u001b[0;4;5;7m");
            Push(d, "underline blink negative");
            Push(d, "\u001b[18;45H");
            Push(d, "\u001b[0;1;4;5;7m");
            Push(d, "bold underline blink negative");
            Push(d, "\u001b[m");
            Push(d, "\u001b[?5l");
            Push(d, "\u001b[23;1H");
            Push(d, "\u001b[0K");
            Push(d, "Dark background.Push<RETURN>");
        }

        [Fact]
        public void TestDarkBackgroundPage()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 24);
            //t.TestPatternScrollingDiagonalLower();

            FirstPage(t, d);
            DarkBackgroundPage(t, d);

            s = t.PageAsSpans;

            Assert.Equal(ExpectedDarkBackgroundPage, s);
        }
    }
}
