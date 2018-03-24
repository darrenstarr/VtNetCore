using System;
using System.Collections.Generic;
using System.Linq;
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

        public static string OSC(this string x, int ps, string text)
        {
            return x + ESC() + ']' + ps.ToString() + ";" + text + "\u0007";
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

        public static string Query(this string x)
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
        public static string STBM(this string x, int top=1, int bottom=1)
        {
            if (top == bottom)
                return x.CSI().T("r");

            return x.CSI().Command(top, bottom, 'r');
        }


        // CSI Pm h Set Mode(SM).
        //     Ps = 4  -> Insert Mode(IRM).
        public static string InsertMode(this string x)
        {
            return x.CSI().Command(4, 'h');
        }

        // CSI Pm l Reset Mode(RM).
        //     Ps = 4  -> Replace Mode(IRM).
        public static string ReplaceMode(this string x)
        {
            return x.CSI().Command(4, 'l');
        }

        // CSI Pm h Set Mode(SM).
        //     Ps = 2 0  -> Automatic Newline(LNM).
        public static string AutomaticNewlineMode(this string x)
        {
            return x.CSI().Command(20, 'h');
        }

        // CSI Pm l Reset Mode(RM).
        //     Ps = 2 0  -> Normal Linefeed (LNM).
        public static string NormalLineFeedMode(this string x)
        {
            return x.CSI().Command(20, 'l');
        }

        // CSI Pl; Pr s
        //   Set left and right margins(DECSLRM), available only when
        //   DECLRMM is enabled(VT420 and up).
        public static string LRMM(this string x, int left, int right)
        {
            return x.CSI().Command(left, right, 's');
        }

        // Ps = 2  -> Designate USASCII for character sets G0-G3 (DECANM), and set VT100 mode.
        public static string SetVt100Mode(this string x)
        {
            return x.CSI().Query().Command(2, 'h');
        }

        // Ps = 2  -> Designate VT52 mode (DECANM).
        public static string SetVt52Mode(this string x)
        {
            return x.CSI().Query().Command(2, 'l');
        }
        // Ps = 7  -> Wraparound Mode (DECAWM).
        public static string EnableWrapAround(this string x)
        {
            return x.CSI().Query().Command(7, 'h');
        }

        // Ps = 7  -> No Wraparound Mode (DECAWM).
        public static string DisableWrapAround(this string x)
        {
            return x.CSI().Query().Command(7, 'l');
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

        // CSI? Pm h - DEC Private Mode Set(DECSET).
        //     Ps = 6->Origin Mode (DECOM).
        public static string EnableDECOM(this string x)
        {
            return x.CSI().Query().Command(6, 'h');
        }

        // CSI? Pm l - DEC Private Mode Reset (DECRST).
        //     Ps = 6  -> Normal Cursor Mode (DECOM).
        public static string DisableDECOM(this string x)
        {
            return x.CSI().Query().Command(6, 'l');
        }

        // CSI? Pm h -DEC Private Mode Set(DECSET).
        //     Ps = 2 5  -> Show Cursor(DECTCEM).
        public static string EnableDECSET(this string x)
        {
            return x.CSI().Query().Command(25, 'h');
        }

        // CSI? Pm l - DEC Private Mode Reset (DECRST).
        //     Ps = 2 5  -> Hide Cursor (DECTCEM).
        public static string DisableDECSET(this string x)
        {
            return x.CSI().Query().Command(25, 'l');
        }

        // CSI? Pm h -DEC Private Mode Set(DECSET).
        //     Ps = 1 2  -> Start Blinking Cursor (AT&T 610).
        public static string StartBlinkingCursor(this string x)
        {
            return x.CSI().Query().Command(12, 'h');
        }

        // CSI? Pm l - DEC Private Mode Reset (DECRST).
        //     Ps = 1 2  -> Stop Blinking Cursor (AT&T 610).
        public static string StopBlinkingCursor(this string x)
        {
            return x.CSI().Query().Command(12, 'l');
        }

        // CSI Ps; Ps H
        //  Cursor Position[row; column] (default = [1,1]) (CUP).
        public static string CUP(this string x, int row = 1, int column = 1)
        {
            if (column == 1 && row == 1)
                return x.CSI().T("H");

            if (column == 1)
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

        // ESC H
        //  Tab Set(HTS  is 0x88).
        public static string HTS(this string x)
        {
            return x + ESC() + "H";
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

        // Tab Clear (TBC).
        //  Ps = 0  -> Clear Current Column(default).
        //  Ps = 3  -> Clear All.
        public static string TBC(this string x, int Ps = 1)
        {
            return x.CSI().Command(Ps, 'g');
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

        // CSI Ps J  Erase in Display (ED).
        //    Ps = 0  -> Erase Below(default).
        //    Ps = 1  -> Erase Above.
        //    Ps = 2  -> Erase All.
        //    Ps = 3->Erase Saved Lines (xterm).
        public static string ED(this string x, int command = 0)
        {
            if (command == 0)
                return x.CSI().T("J");

            return x.CSI().T(command.ToString()).T("J");
        }

        // CSI? Ps$ p - Request DEC private mode(DECRQM).
        //    For VT300 and up, reply is CSI? Ps; Pm$ y
        public static string DecDECRQM(this string x, int mode)
        {
            return x.CSI().Query().T(mode.ToString()).T("$p");
        }

        // CSI Ps SP q - Set cursor style(DECSCUSR, VT520).
        //     Ps = 0  -> blinking block.
        //     Ps = 1  -> blinking block (default).
        //     Ps = 2  -> steady block.
        //     Ps = 3  -> blinking underline.
        //     Ps = 4->steady underline.
        //     Ps = 5->blinking bar (xterm).
        //     Ps = 6->steady bar (xterm).
        public static string DECSCUSR(this string x, int mode)
        {
            return x.CSI().T(mode.ToString()).T(" q");
        }

        public static string DECSCA(this string x, int command = 0)
        {
            if (command == 0)
                return x.CSI().T("\"q");

            return x.CSI().T(command.ToString()).T("\"q");
        }

        // CSI? Ps J - Erase in Display(DECSED).
        //     Ps = 0  -> Selective Erase Below(default).
        //     Ps = 1  -> Selective Erase Above.
        //     Ps = 2  -> Selective Erase All.
        //     Ps = 3  -> Selective Erase Saved Lines(xterm).
        public static string DECSED(this string x, int command = 0)
        {
            if (command == 0)
                return x.CSI().Query().T("J");

            return x.CSI().Query().T(command.ToString()).T("J");
        }

        // CSI? Ps K - Erase in Line(DECSEL).
        //     Ps = 0  -> Selective Erase to Right(default).
        //     Ps = 1  -> Selective Erase to Left.
        //     Ps = 2  -> Selective Erase All.
        public static string DECSEL(this string x, int command = 0)
        {
            if (command == 0)
                return x.CSI().Query().T("K");

            return x.CSI().Query().T(command.ToString()).T("K");
        }

        public static string DesignateG0(this string x, char mode)
        {
            return x + ESC() + "(" + mode;
        }

        public static string DesignateG1(this string x, char mode)
        {
            return x + ESC() + ")" + mode;
        }

        public static string DesignateG2(this string x, char mode)
        {
            return x + ESC() + "*" + mode;
        }

        public static string DesignateG3(this string x, char mode)
        {
            return x + ESC() + "+" + mode;
        }

        public static string SS2(this string x, char mode)
        {
            return x + '\u008e' + mode;
        }

        public static string SS3(this string x, char mode)
        {
            return x + '\u008f' + mode;
        }

        public static string ShiftIn(this string x)
        {
            return x + "\u000f";
        }

        public static string ShiftOut(this string x)
        {
            return x + "\u000e";
        }

        public static string LS2(this string x)
        {
            return x + ESC() + "n";
        }

        public static string LS3(this string x)
        {
            return x + ESC() + "o";
        }

        public static string LS1R(this string x)
        {
            return x + ESC() + "~";
        }

        public static string LS2R(this string x)
        {
            return x + ESC() + "}";
        }

        public static string LS3R(this string x)
        {
            return x + ESC() + "|";
        }

        // ESC # 3   DEC double-height line, top half (DECDHL).
        public static string DECDHLTop(this string x)
        {
            return x + ESC() + "#3";
        }

        // ESC # 4   DEC double-height line, bottom half (DECDHL).
        public static string DECDHLBottom(this string x)
        {
            return x + ESC() + "#4";
        }
        
        // ESC # 5   DEC single-width line (DECSWL).
        public static string DECSWL(this string x)
        {
            return x + ESC() + "#5";
        }

        // ESC # 6   DEC double-width line (DECDWL).
        public static string DECDWL(this string x)
        {
            return x + ESC() + "#6";
        }

        // ESC # 8   DEC Screen Alignment Test (DECALN).
        public static string DECALN(this string x)
        {
            return x + ESC() + "#8";
        }

        // OSC Ps; Pt BEL - Set Text Parameters of VT window.
        //     Ps = 0  -> Change Icon Name and Window Title to Pt.
        //     Ps = 1->Change Icon Name to Pt.
        //     Ps = 2->Change Window Title to Pt.
        public static string ChangeWindowTitle(this string x, string title)
        {
            return x.OSC(2, title);
        }

        // DCS $ q Pt ST
        //   Request Status String (DECRQSS).  The string following the "q"
        //   is one of the following:
        //     " q     -> DECSCA
        //     " p     -> DECSCL
        //     r       -> DECSTBM
        //     s       -> DECSLRM
        //     m       -> SGR
        //     SP q    -> DECSCUSR
        //   xterm responds with DCS 1 $ r Pt ST for valid requests,
        //   replacing the Pt with the corresponding CSI string, or DCS 0 $
        //   r Pt ST for invalid requests.
        public static string DECRQSS(this string x, string mode)
        {
            return x + "\u0090$q" + mode + "\u001b\\";
        }

        // ESC <     Exit VT52 mode (Enter VT100 mode).
        public static string EnterANSIMode(this string x)
        {
            return x + ESC() + "<";
        }

        // ESC A     Cursor up.
        public static string Vt52cuu1(this string x)
        {
            return x + ESC() + "A";
        }

        // ESC B     Cursor down.
        public static string Vt52cud1(this string x)
        {
            return x + ESC() + "B";
        }

        // ESC C     Cursor right.
        public static string Vt52cuf1(this string x)
        {
            return x + ESC() + "C";
        }

        // ESC D     Cursor left.
        public static string Vt52cub1(this string x)
        {
            return x + ESC() + "D";
        }

        // ESC H     Move the cursor to the home position.
        public static string Vt52Home(this string x)
        {
            return x + ESC() + "H";
        }

        // ESC I     Reverse line feed.
        public static string Vt52ri(this string x)
        {
            return x + ESC() + "I";
        }

        // ESC J     Erase from the cursor to the end of the screen.
        public static string Vt52ed(this string x)
        {
            return x + ESC() + "J";
        }

        // ESC K     Erase from the cursor to the end of the line.
        public static string Vt52el(this string x)
        {
            return x + ESC() + "K";
        }

        // ESC Y Ps Ps - Move the cursor to given row and column.
        public static string Vt52cup(this string x, int row, int column)
        {
            return x + ESC() + "Y" + (char)(row + 31) + (char)(column + 31);
        }

        // ESC Z     Identify.
        public static string DECID(this string x)
        {
            return x + ESC() + "Z";
        }

        // CSI Ps ; Ps " p  - Set conformance level (DECSCL).
        //
        // Valid values for the first parameter:
        //     Ps = 6 1  -> VT100.
        //     Ps = 6 2  -> VT200.
        //     Ps = 6 3  -> VT300.
        //
        // Valid values for the second parameter:
        //     Ps = 0  -> 8-bit controls.
        //     Ps = 1  -> 7-bit controls (always set for VT100).
        //     Ps = 2  -> 8-bit controls.
        public static string DECSCL(this string x, int level, int eightBitControl)
        {
            return x.CSI().Command(level, eightBitControl, "\"p");
        }

        public static string vt_hilite(this string x, bool enabled)
        {
            if(enabled)
                return x.CSI().Command(7, "m");

            return x.CSI().T("m");
        }

        public static string ChrPrint2(char ch)
        {
            if(ch <= ' ' || ch >= '\u007F')
                return "  ".vt_hilite(true).T(" <" + ((int)ch).ToString() + "> ").vt_hilite(false);

            return "  ".vt_hilite(true).T(" <" + ch + "> ").vt_hilite(false);
        }

        public static string ChrPrint(this string x, string chrs)
        {
            return string.Join("", chrs.Select(y => ChrPrint2(y)));
        }
    }
}
