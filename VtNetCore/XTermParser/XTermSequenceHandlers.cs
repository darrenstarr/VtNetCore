namespace VtNetCore.XTermParser
{
    using System;
    using System.Linq;
    using VtNetCore.Exceptions;
    using VtNetCore.VirtualTerminal;
    using VtNetCore.VirtualTerminal.Enums;
    using VtNetCore.XTermParser.SequenceType;

    public static class XTermSequenceHandlers
    {
        public static SequenceHandler[] Handlers =
        {
            new SequenceHandler
            {
                SequenceType = SequenceHandler.ESequenceType.Character,
                Handler = (sequence, controller) =>
                {
                    var characterSequence = sequence as CharacterSequence;
                    switch(characterSequence.Character)
                    {
                        case '\n':
                            controller.NewLine();
                            break;
                        case '\r':
                            controller.CarriageReturn();
                            break;
                        case '\b':
                            controller.Backspace();
                            break;
                        case '\u0007':
                            controller.Bell();
                            break;
                        case '\t':
                            controller.Tab();
                            break;
                        case '\u000F':
                            controller.ShiftIn();
                            break;
                        case '\u000E':
                            controller.ShiftOut();
                            break;
                        case '\u000B':
                            controller.VerticalTab();
                            break;
                        case '\f':
                            controller.FormFeed();
                            break;
                        default:
                            controller.PutChar(characterSequence.Character);
                            break;
                    }
                }
            },
            new SequenceHandler
            {
                SequenceType = SequenceHandler.ESequenceType.CharacterSet,
                Handler = (sequence, controller) => controller.SetCharacterSet((sequence as CharacterSetSequence).CharacterSet)
            },
            new SequenceHandler
            {
                Description = "Save Cursor (DECSC)",
                SequenceType = SequenceHandler.ESequenceType.Escape,
                CsiCommand = "7",
                Handler = (sequence, controller) => controller.SaveCursor()
            },
            new SequenceHandler
            {
                Description = "Restore Cursor (DECRC).",
                SequenceType = SequenceHandler.ESequenceType.Escape,
                CsiCommand = "8",
                Handler = (sequence, controller) => controller.RestoreCursor()
            },
            new SequenceHandler
            {
                Description = "Application Keypad (DECKPAM)",
                SequenceType = SequenceHandler.ESequenceType.Escape,
                CsiCommand = "=",
                Handler = (sequence, controller) => controller.SetKeypadType(EKeypadType.Application)
            },
            new SequenceHandler
            {
                Description = "Normal Keypad (DECKPNM).",
                SequenceType = SequenceHandler.ESequenceType.Escape,
                CsiCommand = ">",
                Handler = (sequence, controller) => controller.SetKeypadType(EKeypadType.Normal)
            },
            new SequenceHandler
            {
                Description = "Full Reset (RIS)",
                SequenceType = SequenceHandler.ESequenceType.Escape,
                CsiCommand = "c",
                Handler = (sequence, controller) => controller.FullReset()
            },
            new SequenceHandler
            {
                Description = "Index (IND  is 0x84).",
                SequenceType = SequenceHandler.ESequenceType.Escape,
                CsiCommand = "D",
                Handler = (sequence, controller) => controller.NewLine()
            },
            new SequenceHandler
            {
                Description = "Next Line (NEL  is 0x85).",
                SequenceType = SequenceHandler.ESequenceType.Escape,
                CsiCommand = "E",
                Handler = (sequence, controller) => {
                    controller.CarriageReturn();
                    controller.NewLine();
                }
            },
            new SequenceHandler
            {
                Description = "Tab Set",
                SequenceType = SequenceHandler.ESequenceType.Escape,
                CsiCommand = "H",
                Handler = (sequence, controller) => controller.TabSet()
            },
            new SequenceHandler
            {
                Description = "Reverse Index",
                SequenceType = SequenceHandler.ESequenceType.Escape,
                CsiCommand = "M",
                Handler = (sequence, controller) => controller.ReverseIndex()
            },
            new SequenceHandler
            {
                Description = "DEC double-height line, top half (DECDHL).",
                SequenceType = SequenceHandler.ESequenceType.CharacterSize,
                Handler = (sequence, controller) => controller.SetCharacterSize((sequence as CharacterSizeSequence).Size)
            },
            new SequenceHandler
            {
                Description = "Set Text Parameters (Icon and Title)",
                SequenceType = SequenceHandler.ESequenceType.OSC,
                Param0 = new int [] { 0, 1, 2, 21 },
                Handler = (sequence, controller) => controller.SetWindowTitle(sequence.Command)
            },
            new SequenceHandler
            {
                Description = "Insert Ps (Blank) Character(s) (default = 1)",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "@",
                ExactParameterCountOrDefault = 1,
                Handler = (sequence, controller) => controller.InsertBlanks(((sequence.Parameters == null || sequence.Parameters.Count == 0 || sequence.Parameters[0] == 0) ? 1 : sequence.Parameters[0]))
            },
            new SequenceHandler
            {
                Description = "Character Position Relative  [columns] (default = [row,col+1])",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "a",
                ExactParameterCountOrDefault = 1,
                Handler = (sequence, controller) => {
                    var count = ((sequence.Parameters == null || sequence.Parameters.Count == 0 || sequence.Parameters[0] == 0) ? 1 : sequence.Parameters[0]);
                    controller.MoveCursorRelative(count, 0);
                }
            },
            new SequenceHandler
            {
                Description = "Send Device Attributes (Secondary DA).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                Send = true,
                CsiCommand = "c",
                ExactParameterCountOrDefault = 1,
                Param0 = new int [] { 0 },
                Handler = (sequence, controller) => controller.SendDeviceAttributesSecondary()
            },
            new SequenceHandler
            {
                Description = "Send Device Attributes (Primary DA).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "c",
                ExactParameterCountOrDefault = 1,
                Param0 = new int [] { 0 },
                Handler = (sequence, controller) => controller.SendDeviceAttributes()
            },
            new SequenceHandler
            {
                Description = "Line Position Absolute  [row] (default = [1,column])",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "d",
                ExactParameterCountOrDefault = 1,
                Handler = (sequence, controller) => {
                    var value = ((sequence.Parameters == null || sequence.Parameters.Count == 0 || sequence.Parameters[0] == 0) ? 1 : sequence.Parameters[0]);
                    controller.SetAbsoluteRow(value);
                }
            },
            new SequenceHandler
            {
                Description = "Line Position Relative  [rows] (default = [row+1,column])",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "e",
                ExactParameterCountOrDefault = 1,
                Handler = (sequence, controller) => {
                    var count = ((sequence.Parameters == null || sequence.Parameters.Count == 0 || sequence.Parameters[0] == 0) ? 1 : sequence.Parameters[0]);
                    controller.MoveCursorRelative(0, count);
                }
            },
            new SequenceHandler
            {
                Description = "Horizontal and Vertical Position [row;column]",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "f",
                ExactParameterCountOrDefault = 2,
                Handler = (sequence, controller) =>
                {
                    if(sequence.Parameters.Count == 0)
                        controller.SetCursorPosition(1,1);
                    else
                    {
                        var row = Math.Max(sequence.Parameters[0], 1);
                        var col = Math.Max(sequence.Parameters[1], 1);
                        controller.SetCursorPosition(col, row);
                    }
                }
            },
            new SequenceHandler
            {
                Description = "Tab Clear (TBC) - Clear Current Column (default).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "g",
                ExactParameterCountOrDefault = 1,
                Param0 = new int [] { 0 },
                Handler = (sequence, controller) => controller.ClearTab()
            },
            new SequenceHandler
            {
                Description = "Tab Clear (TBC) - Clear all.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "g",
                ExactParameterCount = 1,
                Param0 = new int [] { 3 },
                Handler = (sequence, controller) => controller.ClearTabs()
            },
            new SequenceHandler
            {
                Description = "Set Mode - Insert Mode",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "h",
                ExactParameterCount = 1,
                Param0 = new int [] { 4 },
                Handler = (sequence, controller) => controller.SetInsertReplaceMode(EInsertReplaceMode.Insert)
            },
            new SequenceHandler
            {
                Description = "Set Mode - Automatic Newline",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "h",
                ExactParameterCount = 1,
                Param0 = new int [] { 20 },
                Handler = (sequence, controller) => controller.SetAutomaticNewLine(true)
            },
            new SequenceHandler
            {
                Description = "Application Cursor Keys",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "h",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 1 },
                Handler = (sequence, controller) => controller.EnableApplicationCursorKeys(true)
            },
            new SequenceHandler
            {
                Description = "132 Column Mode (DECCOLM).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "h",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 3 },
                Handler = (sequence, controller) => controller.Enable132ColumnMode(true)
            },
            new SequenceHandler
            {
                Description = "Smooth (Slow) Scroll (DECSCLM)",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "h",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 4 },
                Handler = (sequence, controller) => controller.EnableSmoothScrollMode(true)
            },
            new SequenceHandler
            {
                Description = "Reverse Video (DECSCNM).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "h",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 5 },
                Handler = (sequence, controller) => controller.EnableReverseVideoMode(true)
            },
            new SequenceHandler
            {
                Description = "Origin Mode (DECOM).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "h",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 6 },
                Handler = (sequence, controller) => controller.EnableOriginMode(true)
            },
            new SequenceHandler
            {
                Description = "Wraparound Mode (DECAWM).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "h",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 7 },
                Handler = (sequence, controller) => controller.EnableWrapAroundMode(true)
            },
            new SequenceHandler
            {
                Description = "Auto-repeat Keys (DECARM).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "h",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 8 },
                Handler = (sequence, controller) => controller.EnableAutoRepeatKeys(true)
            },
            new SequenceHandler
            {
                Description = "Start Blinking Cursor (att610).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "h",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 12 },
                Handler = (sequence, controller) => controller.EnableBlinkingCursor(true)
            },
            new SequenceHandler
            {
                Description = "Show Cursor (DECTCEM).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "h",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 25 },
                Handler = (sequence, controller) => controller.ShowCursor(true)
            },
            new SequenceHandler
            {
                Description = "Allow 80 -> 132 Mode.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "h",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 40 },
                Handler = (sequence, controller) => controller.Enable80132Mode(true)
            },
            new SequenceHandler
            {
                Description = "Reverse-wraparound Mode.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "h",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 45 },
                Handler = (sequence, controller) => controller.EnableReverseWrapAroundMode(true)
            },
            new SequenceHandler
            {
                Description = "Use Alternative Screen Buffer.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "h",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 47 },
                Handler = (sequence, controller) => controller.EnableAlternateBuffer()
            },
            new SequenceHandler
            {
                Description = "Use Hilite Mouse Tracking.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "h",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 1001 },
                Handler = (sequence, controller) => controller.UseHighlightMouseTracking(true)
            },
            new SequenceHandler
            {
                Description = "Use Cell Motion Mouse Tracking.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "h",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 1002 },
                Handler = (sequence, controller) => controller.UseCellMotionMouseTracking(true)
            },
            new SequenceHandler
            {
                Description = "Enable SGR Mouse Mode.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "h",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 1006 },
                Handler = (sequence, controller) => controller.EnableSgrMouseMode(true)
            },
            new SequenceHandler
            {
                Description = "Save cursor as in DECSC and use Alternate Screen Buffer, clearing it first.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "h",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 1049 },
                Handler = (sequence, controller) =>
                {
                    controller.SaveCursor();
                    controller.EnableAlternateBuffer();
                }
            },
            new SequenceHandler
            {
                Description = "Set bracketed paste mode.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "h",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 2004 },
                Handler = (sequence, controller) => controller.SetBracketedPasteMode(true)
            },
            new SequenceHandler
            {
                Description = "Character Position Relative Backwards [columns] (default = [row,col-1])",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "j",
                ExactParameterCountOrDefault = 1,
                Handler = (sequence, controller) => {
                    var count = ((sequence.Parameters == null || sequence.Parameters.Count == 0 || sequence.Parameters[0] == 0) ? 1 : sequence.Parameters[0]);
                    controller.MoveCursorRelative(-count, 0);
                }
            },
            new SequenceHandler
            {
                Description = "Vertical position backwards",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "k",
                ExactParameterCountOrDefault = 1,
                Handler = (sequence, controller) => {
                    var count = ((sequence.Parameters == null || sequence.Parameters.Count == 0 || sequence.Parameters[0] == 0) ? 1 : sequence.Parameters[0]);
                    controller.MoveCursorRelative(0, -count);
                }
            },
            new SequenceHandler
            {
                Description = "Reset Mode - Replace Mode",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "l",
                ExactParameterCount = 1,
                Param0 = new int [] { 4 },
                Handler = (sequence, controller) => controller.SetInsertReplaceMode(EInsertReplaceMode.Replace)
            },
            new SequenceHandler
            {
                Description = "Set Mode - Normal Linefeed",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "l",
                ExactParameterCount = 1,
                Param0 = new int [] { 20 },
                Handler = (sequence, controller) => controller.SetAutomaticNewLine(false)
            },
            new SequenceHandler
            {
                Description = "Normal Cursor Keys",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "l",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 1 },
                Handler = (sequence, controller) => controller.EnableApplicationCursorKeys(false)
            },
            new SequenceHandler
            {
                Description = "80 Column Mode (DECCOLM).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "l",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 3 },
                Handler = (sequence, controller) => controller.Enable132ColumnMode(false)
            },
            new SequenceHandler
            {
                Description = "Jump (Fast) Scroll (DECSCLM).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "l",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 4 },
                Handler = (sequence, controller) => controller.EnableSmoothScrollMode(false)
            },
            new SequenceHandler
            {
                Description = "Normal Video (DECSCNM).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "l",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 5 },
                Handler = (sequence, controller) => controller.EnableReverseVideoMode(false)
            },
            new SequenceHandler
            {
                Description = "Normal Cursor Mode (DECOM).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "l",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 6 },
                Handler = (sequence, controller) => controller.EnableOriginMode(false)
            },
            new SequenceHandler
            {
                Description = "No Wraparound Mode (DECAWM).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "l",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 7 },
                Handler = (sequence, controller) => controller.EnableWrapAroundMode(false)
            },
            new SequenceHandler
            {
                Description = "No Auto-repeat Keys (DECARM).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "l",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 8 },
                Handler = (sequence, controller) => controller.EnableAutoRepeatKeys(false)
            },
            new SequenceHandler
            {
                Description = "Stop Blinking Cursor (att610).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "l",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 12 },
                Handler = (sequence, controller) => controller.EnableBlinkingCursor(false)
            },
            new SequenceHandler
            {
                Description = "Hide Cursor (DECTCEM).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "l",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 25 },
                Handler = (sequence, controller) => controller.ShowCursor(false)
            },
            new SequenceHandler
            {
                Description = "Disallow 80 -> 132 Mode.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "l",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 40 },
                Handler = (sequence, controller) => controller.Enable80132Mode(false)
            },
            new SequenceHandler
            {
                Description = "No Reverse-wraparound Mode.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "l",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 45 },
                Handler = (sequence, controller) => controller.EnableReverseWrapAroundMode(false)
            },
            new SequenceHandler
            {
                Description = "Use Normal Screen Buffer.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "l",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 47 },
                Handler = (sequence, controller) => controller.EnableNormalBuffer()
            },
            new SequenceHandler
            {
                Description = "Don't use Hilite Mouse Tracking.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "l",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 1001 },
                Handler = (sequence, controller) => controller.UseHighlightMouseTracking(false)
            },
            new SequenceHandler
            {
                Description = "Don't use Hilite Mouse Tracking.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "l",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 1002 },
                Handler = (sequence, controller) => controller.UseCellMotionMouseTracking(false)
            },
            new SequenceHandler
            {
                Description = "Disable SGR Mouse Mode.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "l",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 1006 },
                Handler = (sequence, controller) => controller.EnableSgrMouseMode(false)
            },
            new SequenceHandler
            {
                Description = "Use Normal Screen Buffer and restore cursor as in DECRC",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "l",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 1049 },
                Handler = (sequence, controller) =>
                {
                    controller.RestoreCursor();
                    controller.EnableNormalBuffer();
                }
            },
            new SequenceHandler
            {
                Description = "Reset bracketed paste mode.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "l",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 2004 },
                Handler = (sequence, controller) => controller.SetBracketedPasteMode(false)
            },
            new SequenceHandler
            {
                Description = "Character Attributes (SGR).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "m",
                Handler = (sequence, controller) =>
                {
                    var csiSequence = sequence as CsiSequence;
                    if(csiSequence.Parameters.Count == 0)
                        controller.SetCharacterAttribute(0);
                    else
                    {
                        foreach(var parameter in csiSequence.Parameters)
                            controller.SetCharacterAttribute(parameter);
                    }
                }
            },
            new SequenceHandler
            {
                Description = "Device Status Report (DSR). - Status Report.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "n",
                ExactParameterCount = 1,
                Param0 = new int [] { 5 },
                Handler = (sequence, controller) => controller.DeviceStatusReport()
            },
            new SequenceHandler
            {
                Description = "Device Status Report (DSR). - Report Cursor Position (CPR)",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "n",
                ExactParameterCount = 1,
                Param0 = new int [] { 6 },
                Handler = (sequence, controller) => controller.ReportCursorPosition()
            },
            new SequenceHandler
            {
                Description = "Soft terminal reset (DECSTR).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "p",
                Bang = true,
                Handler = (sequence, controller) => controller.FullReset()
            },
            new SequenceHandler
            {
                Description = "Set Scrolling Region [top;bottom] (default = full size of window)",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "r",
                ExactParameterCountOrDefault = 2,
                Handler = (sequence, controller) =>
                {
                    if(sequence.Parameters.Count == 0)
                        controller.ClearScrollingRegion();
                    else if(sequence.Parameters.Count == 2)
                        controller.SetScrollingRegion(sequence.Parameters[0], sequence.Parameters[1]);
                }
            },
            new SequenceHandler
            {
                Description = "Restore Cursor Keys",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "r",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 1 },
                Handler = (sequence, controller) => controller.RestoreCursorKeys()
            },
            new SequenceHandler
            {
                Description = "Restore Normal Screen Buffer.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "r",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 47 },
                Handler = (sequence, controller) => controller.RestoreEnableNormalBuffer()
            },
            new SequenceHandler
            {
                Description = "Restore Hilite Mouse Tracking.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "r",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 1001 },
                Handler = (sequence, controller) => controller.RestoreUseHighlightMouseTracking()
            },
            new SequenceHandler
            {
                Description = "Restore use Hilite Mouse Tracking.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "r",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 1002 },
                Handler = (sequence, controller) => controller.RestoreUseCellMotionMouseTracking()
            },
            new SequenceHandler
            {
                Description = "Restore SGR Mouse Mode.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "r",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 1006 },
                Handler = (sequence, controller) => controller.RestoreEnableSgrMouseMode()
            },
            new SequenceHandler
            {
                Description = "Restore bracketed paste mode.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "r",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 2004 },
                Handler = (sequence, controller) => controller.RestoreBracketedPasteMode()
            },
            new SequenceHandler
            {
                Description = "Save Cursor Keys",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "s",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 1 },
                Handler = (sequence, controller) => controller.SaveCursorKeys()
            },
            new SequenceHandler
            {
                Description = "Save Normal Screen Buffer.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "s",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 47 },
                Handler = (sequence, controller) => controller.SaveEnableNormalBuffer()
            },
            new SequenceHandler
            {
                Description = "Save Hilite Mouse Tracking.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "s",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 1001 },
                Handler = (sequence, controller) => controller.SaveUseHighlightMouseTracking()
            },
            new SequenceHandler
            {
                Description = "Save use Hilite Mouse Tracking.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "s",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 1002 },
                Handler = (sequence, controller) => controller.SaveUseCellMotionMouseTracking()
            },
            new SequenceHandler
            {
                Description = "Save SGR Mouse Mode.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "s",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 1006 },
                Handler = (sequence, controller) => controller.SaveEnableSgrMouseMode()
            },
            new SequenceHandler
            {
                Description = "Save bracketed paste mode.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "s",
                Query = true,
                ExactParameterCount = 1,
                Param0 = new int [] { 2004 },
                Handler = (sequence, controller) => controller.SaveBracketedPasteMode()
            },
            new SequenceHandler
            {
                Description = "Cursor Up Ps Times (default = 1)",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "A",
                ExactParameterCountOrDefault = 1,
                Handler = (sequence, controller) =>
                {
                    controller.MoveCursorRelative(0, -((sequence.Parameters == null || sequence.Parameters.Count == 0 || sequence.Parameters[0] == 0) ? 1 : sequence.Parameters[0]));
                }
            },
            new SequenceHandler
            {
                Description = "Cursor Down Ps Times (default = 1)",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "B",
                ExactParameterCountOrDefault = 1,
                Handler = (sequence, controller) =>
                {
                    controller.MoveCursorRelative(0, ((sequence.Parameters == null || sequence.Parameters.Count == 0 || sequence.Parameters[0] == 0) ? 1 : sequence.Parameters[0]));
                }
            },
            new SequenceHandler
            {
                Description = "Cursor Forward Ps Times (default = 1)",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "C",
                ExactParameterCountOrDefault = 1,
                Handler = (sequence, controller) =>
                {
                    controller.MoveCursorRelative(((sequence.Parameters == null || sequence.Parameters.Count == 0 || sequence.Parameters[0] == 0) ? 1 : sequence.Parameters[0]), 0);
                }
            },
            new SequenceHandler
            {
                Description = "Cursor Backward Ps Times (default = 1)",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "D",
                ExactParameterCountOrDefault = 1,
                Handler = (sequence, controller) =>
                {
                    controller.MoveCursorRelative(-((sequence.Parameters == null || sequence.Parameters.Count == 0 || sequence.Parameters[0] == 0) ? 1 : sequence.Parameters[0]), 0);
                }
            },
            new SequenceHandler
            {
                Description = "Cursor Next Line Ps Times (default = 1)",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "E",
                ExactParameterCountOrDefault = 1,
                Handler = (sequence, controller) => {
                    var count = ((sequence.Parameters == null || sequence.Parameters.Count == 0 || sequence.Parameters[0] == 0) ? 1 : sequence.Parameters[0]);
                    controller.CarriageReturn();
                    controller.MoveCursorRelative(0, count);
                }
            },
            new SequenceHandler
            {
                Description = "Cursor Preceding Line Ps Times (default = 1)",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "F",
                ExactParameterCountOrDefault = 1,
                Handler = (sequence, controller) => {
                    var count = ((sequence.Parameters == null || sequence.Parameters.Count == 0 || sequence.Parameters[0] == 0) ? 1 : sequence.Parameters[0]);
                    controller.CarriageReturn();
                    while((count--) > 0)
                        controller.ReverseIndex();
                }
            },
            new SequenceHandler
            {
                Description = "Cursor Character Absolute  [column] (default = [row,1])",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "G",
                ExactParameterCountOrDefault = 1,
                Handler = (sequence, controller) => {
                    var count = ((sequence.Parameters == null || sequence.Parameters.Count == 0 || sequence.Parameters[0] == 0) ? 1 : sequence.Parameters[0]);
                    controller.CarriageReturn();
                    controller.MoveCursorRelative(count - 1, 0);
                }
            },
            new SequenceHandler
            {
                Description = "Cursor Position [row;column] (default = [1,1])",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "H",
                ExactParameterCountOrDefault = 2,
                Handler = (sequence, controller) =>
                {
                    if(sequence.Parameters == null || sequence.Parameters.Count == 0)
                        controller.SetCursorPosition(1,1);
                    else
                        controller.SetCursorPosition(sequence.Parameters[1], sequence.Parameters[0]);
                }
            },
            new SequenceHandler
            {
                Description = "Cursor Position [row;1]",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "H",
                ExactParameterCount = 1,
                Handler = (sequence, controller) => controller.SetCursorPosition(1, sequence.Parameters[0])
            },
            new SequenceHandler
            {
                Description = "Cursor Forward Tabulation Ps tab stops (default = 1)",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "I",
                ExactParameterCountOrDefault = 1,
                Handler = (sequence, controller) => {
                    var count = ((sequence.Parameters == null || sequence.Parameters.Count == 0 || sequence.Parameters[0] == 0) ? 1 : sequence.Parameters[0]);
                    while((count--) > 0)
                        controller.Tab();
                }
            },
            new SequenceHandler
            {
                Description = "Erase in Display - Selective Erase Below (default)",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "J",
                ExactParameterCountOrDefault = 1,
                Param0 = new int [] { 0 },
                Handler = (sequence, controller) => controller.EraseBelow()
            },
            new SequenceHandler
            {
                Description = "Erase in Display - Selective Erase Above.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "J",
                ExactParameterCount = 1,
                Param0 = new int [] { 1 },
                Handler = (sequence, controller) => controller.EraseAbove()
            },
            new SequenceHandler
            {
                Description = "Erase in Display - Selective Erase All.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "J",
                ExactParameterCount = 1,
                Param0 = new int [] { 2 },
                Handler = (sequence, controller) => controller.EraseAll()
            },
            new SequenceHandler
            {
                Description = "Erase in Line - Selective Erase to Right (default)",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "K",
                ExactParameterCountOrDefault = 1,
                Param0 = new int [] { 0 },
                Handler = (sequence, controller) => controller.EraseToEndOfLine()
            },
            new SequenceHandler
            {
                Description = "Erase in Line - Selective Erase to Left.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "K",
                ExactParameterCount = 1,
                Param0 = new int [] { 1 },
                Handler = (sequence, controller) => controller.EraseToStartOfLine()
            },
            new SequenceHandler
            {
                Description = "Erase in Line - Selective Erase All.",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "K",
                ExactParameterCount = 1,
                Param0 = new int [] { 2 },
                Handler = (sequence, controller) => controller.EraseLine()
            },
            new SequenceHandler
            {
                Description = "Insert Ps Line(s) (default = 1) (IL).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "L",
                ExactParameterCountOrDefault = 1,
                Handler = (sequence, controller) => controller.InsertLines((sequence.Parameters == null || sequence.Parameters.Count == 0) ? 1 : sequence.Parameters[0])
            },
            new SequenceHandler
            {
                Description = "Delete Ps Line(s) (default = 1) (DL).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "M",
                ExactParameterCountOrDefault = 1,
                Handler = (sequence, controller) => controller.DeleteLines((sequence.Parameters == null || sequence.Parameters.Count == 0) ? 1 : sequence.Parameters[0])
            },
            new SequenceHandler
            {
                Description = "Delete Ps Character(s) (default = 1)",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "P",
                ExactParameterCountOrDefault = 1,
                Handler = (sequence, controller) =>
                {
                    controller.DeleteCharacter((sequence.Parameters == null || sequence.Parameters.Count == 0) ? 1 : sequence.Parameters[0]);
                }
            },
            new SequenceHandler
            {
                Description = "Cursor Backward Tabulation Ps tab stops (default = 1) (CBT).",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "Z",
                ExactParameterCountOrDefault = 1,
                Handler = (sequence, controller) => {
                    var count = ((sequence.Parameters == null || sequence.Parameters.Count == 0 || sequence.Parameters[0] == 0) ? 1 : sequence.Parameters[0]);
                    while((count--) > 0)
                        controller.ReverseTab();
                }
            },
            new SequenceHandler
            {
                Description = "Character Position Absolute  [column] (default = [row,1])",
                SequenceType = SequenceHandler.ESequenceType.CSI,
                CsiCommand = "`",
                ExactParameterCountOrDefault = 1,
                Handler = (sequence, controller) => {
                    var count = ((sequence.Parameters == null || sequence.Parameters.Count == 0 || sequence.Parameters[0] == 0) ? 1 : sequence.Parameters[0]);
                    controller.CarriageReturn();
                    controller.MoveCursorRelative(count - 1, 0);
                }
            },
            new SequenceHandler
            {
                Description = "Select default character set.  That is ISO 8859-1",
                SequenceType = SequenceHandler.ESequenceType.Unicode,
                CsiCommand = "@",
                Handler = (sequence, controller) => controller.SetLatin1()
            },
            new SequenceHandler
            {
                Description = "Select UTF-8 character set (ISO 2022).",
                SequenceType = SequenceHandler.ESequenceType.Unicode,
                CsiCommand = "G",
                Handler = (sequence, controller) => controller.SetUTF8()
            },
        };

        public static void ProcessSequence(TerminalSequence sequence, IVirtualTerminalController controller)
        {
            if(sequence is CharacterSequence)
            {
                var handler = Handlers.Where(x => x.SequenceType == SequenceHandler.ESequenceType.Character).SingleOrDefault();
                if (handler == null)
                    throw new Exception("There are no sequence handlers configured for type CharacterSequence");

                handler.Handler(sequence, controller);

                return;
            }

            if (sequence is CsiSequence)
            {
                var handler = Handlers
                    .Where(x =>
                        x.SequenceType == SequenceHandler.ESequenceType.CSI &&
                        x.CsiCommand == sequence.Command &&
                        x.Query == sequence.IsQuery &&
                        x.Send == sequence.IsSend &&
                        x.Bang == sequence.IsBang &&
                        (
                            x.Param0.Length == 0 ||
                            x.Param0.Contains(
                                (sequence.Parameters == null || sequence.Parameters.Count < 1) ?
                                0 :
                                sequence.Parameters[0]
                            )
                        ) &&
                        (
                            (
                                x.ExactParameterCount == -1 &&
                                x.ExactParameterCountOrDefault == -1
                            ) ||
                            (
                                x.ExactParameterCount != -1 &&
                                (sequence.Parameters != null && x.ExactParameterCount == sequence.Parameters.Count)
                            ) ||
                            (
                                x.ExactParameterCountOrDefault != -1 &&
                                (
                                    sequence.Parameters == null ||
                                    sequence.Parameters.Count == 0 ||
                                    x.ExactParameterCountOrDefault == sequence.Parameters.Count
                                )
                            )
                        )
                    )
                    .SingleOrDefault();

                if (handler == null)
                    throw new Exception("There are no CsiSequence handlers configured for sequence: " + sequence.ToString());

                handler.Handler(sequence, controller);

                return;
            }

            if (sequence is OscSequence)
            {
                if (sequence.Parameters.Count < 1)
                    throw new Exception("OSC sequence doesn't have any parameters");

                var handler = Handlers.Where(x => x.SequenceType == SequenceHandler.ESequenceType.OSC && x.Param0.Contains(sequence.Parameters[0])).SingleOrDefault();
                if (handler == null)
                    throw new Exception("There are no sequence handlers configured for type OscSequence with param0 = " + sequence.Parameters[0].ToString());

                handler.Handler(sequence, controller);

                return;
            }

            if (sequence is CharacterSetSequence)
            {
                var handler = Handlers.Where(x => x.SequenceType == SequenceHandler.ESequenceType.CharacterSet).SingleOrDefault();
                if (handler == null)
                    throw new EscapeSequenceException("There are no sequence handlers configured for type CharacterSetSequence " + sequence.ToString(), sequence);

                handler.Handler(sequence, controller);

                return;
            }

            if (sequence is EscapeSequence)
            {
                var handler = Handlers
                    .Where(x => 
                        x.SequenceType == SequenceHandler.ESequenceType.Escape &&
                        x.CsiCommand == sequence.Command
                    )
                    .SingleOrDefault();

                if (handler == null)
                    throw new EscapeSequenceException("There are no sequence handlers configured for type EscapeSequence " + sequence.ToString(), sequence);

                handler.Handler(sequence, controller);

                return;
            }

            if (sequence is CharacterSizeSequence)
            {
                var handler = Handlers.Where(x => x.SequenceType == SequenceHandler.ESequenceType.CharacterSize).SingleOrDefault();
                if (handler == null)
                    throw new EscapeSequenceException("There are no sequence handlers configured for type CharacterSizeSequence " + sequence.ToString(), sequence);

                handler.Handler(sequence, controller);

                return;
            }

            if (sequence is UnicodeSequence)
            {
                var handler = Handlers
                    .Where(x =>
                        x.SequenceType == SequenceHandler.ESequenceType.Unicode &&
                        x.CsiCommand == sequence.Command
                    )
                    .SingleOrDefault();

                if (handler == null)
                    throw new EscapeSequenceException("There are no sequence handlers configured for type UnicodeSequence " + sequence.ToString(), sequence);

                handler.Handler(sequence, controller);

                return;
            }

            System.Diagnostics.Debug.WriteLine("Unhandled sequence -> " + sequence.ToString());
        }
    }
}
