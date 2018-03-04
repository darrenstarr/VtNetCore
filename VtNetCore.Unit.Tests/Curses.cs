using System;
using System.Collections.Generic;
using System.Text;

namespace VtNetCoreUnitTests
{
    public static class Curses
    {
        public static string ESC()
        {
            return "\u001b";
        }

        public static string CSI(this string x)
        {
            return x + ESC() + '[';
        }

        private static string Command(this string x, int a, int b, char c)
        {
            return x + a.ToString() + ";" + b.ToString() + c;
        }

        private static string Command(this string x, int a, char c)
        {
            return x + a.ToString() + c;
        }

        private static string Query(this string x)
        {
            return x + "?";
        }

        public static string LF(this string x, int count=1)
        {
            return x + string.Empty.PadLeft(count, '\n');
        }

        // CSI Ps ; Ps r
        //   Set Scrolling Region [top;bottom] (default = full size of win-
        //   dow) (DECSTBM).
        public static string STBM(this string x, int top, int bottom)
        {
            return x.CSI().Command(top, bottom, 'r');
        }

        // CSI Pl; Pr s
        //   Set left and right margins(DECSLRM), available only when
        //   DECLRMM is enabled(VT420 and up).
        public static string LRMM(this string x, int left, int right)
        {
            return x.CSI().Command(left, right, 's');
        }

        // Ps = 6 9  -> Enable left and right margin mode (DECLRMM)
        public static string EnableLRMM(this string x)
        {
            return x.CSI().Query().Command(69, 'h');
        }

        // Ps = 6 9  -> Disable left and right margin mode (DECLRMM)
        public static string DisableLRMM(this string x)
        {
            return x.CSI().Query().Command(69, 'l');
        }

        // CSI Ps; Ps H
        //  Cursor Position[row; column] (default = [1,1]) (CUP).
        public static string CUP(this string x, int row, int column=1)
        {
            return x.CSI().Command(row, column, 'H');
        }

        // ESC M
        //  Reverse Index(RI  is 0x8d).
        public static string RI(this string x)
        {
            return x + ESC() + "M";
        }

        // ESC D
        //  Index(IND  is 0x84).
        public static string IND(this string x)
        {
            return x + ESC() + "D";
        }

        // CSI Ps S  Scroll up Ps lines (default = 1) (SU).
        public static string SU(this string x, int rows = 1)
        {
            return x.CSI().Command(rows, 'S');
        }

        // CSI Ps T  Scroll down Ps lines (default = 1) (SD).
        public static string SD(this string x, int rows = 1)
        {
            return x.CSI().Command(rows, 'T');
        }
    }
}
