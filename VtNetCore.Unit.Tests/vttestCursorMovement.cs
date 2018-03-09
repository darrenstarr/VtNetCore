namespace VtNetCoreUnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using VtNetCore.VirtualTerminal;
    using VtNetCore.XTermParser;
    using Xunit;

    // The initial menu from vttest
    // \033[?1l
    // \033[?3l
    // \033[?4l
    // \033[?5l
    // \033[?6l
    // \033[?7h
    // \033[?8l
    // \033[?40h
    // \033[?45l
    // \033[r
    // \033[0m
    // \033[2J
    // \033[3;10H
    // VT100 test program, version 2.7 (20140305)
    // \033[4;10H
    // \033[5;10H
    // Choose test type:\r

    public class VttestCursorMovement
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
        public static readonly string ExpectAutoWrap80 =
            "Test of autowrap, mixing control and print characters.                          " + "\n" +
            "The left/right margins should have letters in order:                            " + "\n" +
            "I                                                                              i" + "\n" +
            "J                                                                              j" + "\n" +
            "K                                                                              k" + "\n" +
            "L                                                                              l" + "\n" +
            "M                                                                              m" + "\n" +
            "N                                                                              n" + "\n" +
            "O                                                                              o" + "\n" +
            "P                                                                              p" + "\n" +
            "Q                                                                              q" + "\n" +
            "R                                                                              r" + "\n" +
            "S                                                                              s" + "\n" +
            "T                                                                              t" + "\n" +
            "U                                                                              u" + "\n" +
            "V                                                                              v" + "\n" +
            "W                                                                              w" + "\n" +
            "X                                                                              x" + "\n" +
            "Y                                                                              y" + "\n" +
            "Z                                                                              z" + "\n" +
            "                                                                                " + "\n" +
            "Push <RETURN>                                                                   " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                ";

        [Fact]
        public void AutoWrap80()
        {
            string s;
            var t = new VirtualTerminalController();
            t.Debugging = true;
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);

            Push(d, "Test of autowrap, mixing control and print characters.\r");
            Push(d, "\r\n");
            Push(d, "The left/right margins should have letters in order:\r");
            Push(d, "\r\n");
            Push(d, "".STBM(3, 21).EnableDECOM());

            for (char ch = 'A'; ch <= 'Z'; ch++)
            {
                switch ((ch - 'A') % 4)
                {
                    case 0:
                        Push(d, "".CUP(19, 1).T(ch.ToString()).CUP(19, 80).T(char.ToLower(ch).ToString()).CR().LF());
                        Push(d, "".CUP(18, 80).T(char.ToLower(ch).ToString()));
                        break;
                    case 1:
                        Push(d, ch.ToString().CUP(19, 80).T(char.ToLower(ch).ToString() + "\b \b").CR().LF());
                        break;
                    case 2:
                        Push(d, "".CUP(19, 80).T(ch.ToString() + "\b\b\t\t" + char.ToLower(ch).ToString()));
                        Push(d, "".CUP(19, 2).T("\b" + ch.ToString()).CR().LF());
                        break;
                    case 3:
                        Push(d, "".CUP(19, 80).CR().LF());
                        Push(d, "".CUP(18, 1).T(ch.ToString()).CUP(18, 80).T(char.ToLower(ch).ToString()));
                        break;
                }
            }

            Push(d, "".DisableDECOM().STBM().CUP(22, 1).T("Push <RETURN>"));

            s = t.GetScreenText();
            Assert.Equal(ExpectAutoWrap80, s);
        }

    }
}
