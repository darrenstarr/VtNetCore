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

        public static string T(this string x, string text)
        {
            return x + text;
        }

        private static string Command(this string x, int a, int b, char c)
        {
            if (a == 1)
                return x + ";" + b.ToString() + c;

            return x + a.ToString() + ";" + b.ToString() + c;
        }

        private static string Command(this string x, int a, int b, string c)
        {
            if (a == 1)
                return x + ";" + b.ToString() + c;

            return x + a.ToString() + ";" + b.ToString() + c;
        }

        private static string Command(this string x, int a, char c)
        {
            if (a == 1)
                return x + c;

            return x + a.ToString() + c;
        }

        private static string Command(this string x, int a, string c)
        {
            if (a == 1)
                return x + c;

            return x + a.ToString() + c;
        }

        private static string Query(this string x)
        {
            return x + "?";
        }

        public static string LF(this string x, int count = 1)
        {
            return x + string.Empty.PadLeft(count, '\n');
        }

        public static string CR(this string x, int count = 1)
        {
            return x + string.Empty.PadLeft(count, '\r');
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
        public static string CUP(this string x, int row=1, int column=1)
        {
            if (column == 1 && row == 1)
                return x.CSI().T("H");

            if(column == 1)
                return x.CSI().Command(row, 'H');

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

        // ESC E
        //  Next Line(NEL  is 0x85).
        public static string NEL(this string x)
        {
            return x + ESC() + "E";
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

        // CSI Ps A  Cursor Up Ps Times (default = 1) (CUU).
        public static string CUU(this string x, int count = 1)
        {
            return x.CSI().Command(count, 'A');
        }

        // CSI Ps B  Cursor Down Ps Times (default = 1) (CUD).
        public static string CUD(this string x, int count = 1)
        {
            return x.CSI().Command(count, 'B');
        }

        // CSI Ps C  Cursor Forward Ps Times (default = 1) (CUF).
        public static string CUF(this string x, int count = 1)
        {
            return x.CSI().Command(count, 'C');
        }

        // CSI Ps D  Cursor Backward Ps Times (default = 1) (CUB).
        public static string CUB(this string x, int count = 1)
        {
            return x.CSI().Command(count, 'D');
        }

        // CSI Ps E  Cursor Next Line Ps Times (default = 1) (CNL).
        public static string CNL(this string x, int count = 1)
        {
            return x.CSI().Command(count, 'E');
        }

        // CSI Ps F  Cursor Preceding Line Ps Times (default = 1) (CPL).
        public static string CPL(this string x, int count = 1)
        {
            return x.CSI().Command(count, 'F');
        }

        // CSI Ps G  Cursor Character Absolute  [column] (default = [row,1]) (CHA).
        public static string CHA(this string x, int count = 1)
        {
            return x.CSI().Command(count, 'G');
        }

        // CSI Pm `  Character Position Absolute  [column] (default = [row,1])
        public static string HPA(this string x, int count = 1)
        {
            return x.CSI().Command(count, '`');
        }

        // Character Position Relative  [columns] (default = [row,col+1]) (HPR).
        public static string HPR(this string x, int count = 1)
        {
            return x.CSI().Command(count, 'a');
        }

        // Character Position Backward [columns] (default = [row,col+1]) (HPB).
        public static string HPB(this string x, int count = 1)
        {
            return x.CSI().Command(count, 'j');
        }

        // CSI Ps ; Ps f
        //  Horizontal and Vertical Position[row; column] (default = [1,1]) (HVP).
        public static string HVP(this string x, int row = 1, int column = 1)
        {
            if (column == 1 && row == 1)
                return x.CSI().T("f");

            if (column == 1)
                return x.CSI().Command(row, 'f');

            return x.CSI().Command(row, column, 'f');
        }

        // CSI Pm d  Line Position Absolute  [row] (default = [1,column]) (VPA).
        public static string VPA(this string x, int count = 1)
        {
            return x.CSI().Command(count, 'd');
        }

        // CSI Pm e  Line Position Relative  [rows] (default = [row+1,column]) (VPR).
        public static string VPR(this string x, int count = 1)
        {
            return x.CSI().Command(count, 'e');
        }

        // CSI Pm k  Line Position Backward [rows] (default = [row+1,column]) (VPB).
        public static string VPB(this string x, int count = 1)
        {
            return x.CSI().Command(count, 'k');
        }

        // CSI Ps I  Cursor Forward Tabulation Ps tab stops (default = 1) (CHT).
        public static string CHT(this string x, int count = 1)
        {
            return x.CSI().Command(count, 'I');
        }

        // CSI Ps Z  Cursor Backward Tabulation Ps tab stops (default = 1) (CBT).
        public static string CBT(this string x, int count = 1)
        {
            return x.CSI().Command(count, 'Z');
        }

        // ESC c     Full Reset (RIS).
        public static string RIS(this string x)
        {
            return x + ESC() + "c";
        }

        // CSI Ps @  Insert Ps (Blank) Character(s) (default = 1) (ICH).
        public static string ICH(this string x, int count = 1)
        {
            return x.CSI().Command(count, '@');
        }

        // CSI Ps P  Delete Ps Character(s) (default = 1) (DCH).
        public static string DCH(this string x, int count = 1)
        {
            return x.CSI().Command(count, 'P');
        }

        // CSI Ps X  Erase Ps Character(s) (default = 1) (ECH).
        public static string ECH(this string x, int count = 1)
        {
            return x.CSI().Command(count, 'X');
        }

        // CSI Ps L  Insert Ps Line(s) (default = 1) (IL).
        public static string IL(this string x, int count = 1)
        {
            return x.CSI().Command(count, 'L');
        }

        // CSI Ps M  Delete Ps Line(s) (default = 1) (DL).
        public static string DL(this string x, int count = 1)
        {
            return x.CSI().Command(count, 'M');
        }

        // CSI Pm ' }
        //   Insert Ps Column(s) (default = 1) (DECIC), VT420 and up.
        public static string DECIC(this string x, int count = 1)
        {
            return x.CSI().Command(count, "'}");
        }

        // CSI Pm ' ~
        //  Delete Ps Column(s) (default = 1) (DECDC), VT420 and up.
        public static string DECDC(this string x, int count = 1)
        {
            return x.CSI().Command(count, "'~");
        }

        // CSI Ps K Erase in Line(EL).
        //     Ps = 0  -> Erase to Right(default).
        //     Ps = 1  -> Erase to Left.
        //     Ps = 2  -> Erase All.
        public static string EL(this string x, int command = 0)
        {
            if (command == 0)
                return x.CSI().T("K");

            return x.CSI().T(command.ToString()).T("K");
        }
    }
}
