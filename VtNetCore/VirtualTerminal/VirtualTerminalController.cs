namespace VtNetCore.VirtualTerminal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using VtNetCore.VirtualTerminal.Encodings;
    using VtNetCore.VirtualTerminal.Enums;
    using VtNetCore.VirtualTerminal.Model;

    /// <summary>
    /// Implementation of the buffer manipulation features needed to support a virtual terminal emulator
    /// </summary>
    public class VirtualTerminalController : IVirtualTerminalController
    {
        private TerminalLines alternativeBuffer = new TerminalLines();
        private TerminalLines normalBuffer = new TerminalLines();

        private int alternativeBufferTopRow = 0;
        private int normalBufferTopRow = 0;

        /// <summary>
        /// Configures the maximum number of lines stored in the history
        /// </summary>
        /// <remarks>
        /// This value is exclusive of the active screen area.
        /// </remarks>
        public int MaximumHistoryLines { get; set; } = 2001;

        /// <summary>
        /// Defines the attributes which should be assigned to null character values
        /// </summary>
        /// <remarks>
        /// When drawing the background of the terminal, this attribute should be used to calculate
        /// the value of the color. The colors set here are the colors which were applied during the
        /// last screen erase.
        /// </remarks>
        public TerminalAttribute NullAttribute = new TerminalAttribute();

        /// <summary>
        /// The current buffer
        /// </summary>
        internal TerminalLines Buffer { get; set; }

        private EActiveBuffer ActiveBuffer { get; set; } = EActiveBuffer.Normal;

        /// <summary>
        /// The logical top row of the view port. This translates relative to the buffer
        /// </summary>
        internal int TopRow { get; set; } = 0;

        /// <summary>
        /// The number of logical columns for text formatting
        /// </summary>
        internal int Columns { get; set; } = 80;

        /// <summary>
        /// The number of logical rows for text formatting
        /// </summary>
        internal int Rows { get; set; } = 24;

        /// <summary>
        /// The number of visible columns configured by the hosting application
        /// </summary>
        public int VisibleColumns { get; set; } = 0;

        /// <summary>
        /// The number of visible rows configured by the hosting application
        /// </summary>
        public int VisibleRows { get; set; } = 0;

        /// <summary>
        /// Returns the logical bottom row of the buffer.
        /// </summary>
        /// <remarks>
        /// This is either the top row plus the number of lines or the last row of the buffer,
        /// whichever is greater.
        /// </remarks>
        /// <returns>What should be the absolute maximum bottom row value</returns>
        public int BottomRow
        {
            get
            {
                return Math.Max(Buffer.Count, TopRow + Rows - 1);
            }
        }

        private TerminalCursorState SavedCursorState { get; set; } = null;

        /// <summary>
        /// The current state of all cursor and attribute properties
        /// </summary>
        public TerminalCursorState CursorState { get; set; } = new TerminalCursorState();

        public bool HighlightMouseTracking { get; set; }

        /// <summary>
        /// Enables sending mouse events including press, release and move only when a button is pressed.
        /// </summary>
        public bool CellMotionMouseTracking { get; set; }

        /// <summary>
        /// Enables SGR (Select Graphic Rendition) mouse mode
        /// </summary>
        public bool SgrMouseMode { get; set; }

        /// <summary>
        /// Enables URXVT Mouse mode
        /// </summary>
        public bool UrxvtMouseMode { get; set; }

        /// <summary>
        /// Informs the server when the terminal has focus or not
        /// </summary>
        public bool SendFocusInAndFocusOutEvents { get; set; }

        /// <summary>
        /// Enables sending mouse events including press, release and move even when no button is pressed.
        /// </summary>
        public bool UseAllMouseTracking { get; set; }

        /// <summary>
        /// Signifies that mouse pointer locations should be transmitted as UTF-8 text allowing extents past column 255
        /// </summary>
        public bool Utf8MouseMode { get; set; }

        /// <summary>
        /// Encapsulates pasted text so that receiving applications know it was explicitly pasted.
        /// </summary>
        public bool BracketedPasteMode { get; set; }

        /// <summary>
        /// X10 Protocol send mouse XY on button press
        /// </summary>
        public bool X10SendMouseXYOnButton { get; set; }

        /// <summary>
        /// X11 Protocol send mouse XY on button press
        /// </summary>
        public bool X11SendMouseXYOnButton { get; set; }

        /// <summary>
        /// Guarded text area range
        /// </summary>
        public TextRange GuardedArea { get; set; }

        /// <summary>
        /// Erasure mode (ERM)
        /// </summary>
        public bool ErasureMode { get; set; }

        /// <summary>
        /// Guarded Area Transfer Mode
        /// </summary>
        public bool GuardedAreaTransferMode { get; set; }

        /// <summary>
        /// Smooth scroll Mode (DECSCLM)
        /// </summary>
        public bool SmoothScrollMode { get; set; }

        /// <summary>
        /// Reverse wrap around mode
        /// </summary>
        public bool ReverseWrapAroundMode { get; set; }

        /// <summary>
        /// Set to true when Vt52 Mode is enabled.
        /// </summary>
        /// <remarks>
        /// This is necessary for contextual parsing of input streams as well as supporting VT52 keystrokes
        /// and terminal requests.
        /// </remarks>
        public bool Vt52Mode { get; set; }

        /// <summary>
        /// Implements the IsVt52Mode interface from IVirtualTerminalController
        /// </summary>
        /// <returns>true when the terminal is in Vt52 Mode</returns>
        public bool IsVt52Mode()
        {
            return Vt52Mode;
        }

        /// <summary>
        /// Specifies whetehr VT52 ANSI Mode has been entered.
        /// </summary>
        public bool Vt52AnsiMode { get; set; }

        /// <summary>
        /// When enabled causes logging to System.Diagnostics.Debug
        /// </summary>
        public bool Debugging { get; set; }

        /// <summary>
        /// Specifies the visual scrolling region top in base 0
        /// </summary>
        public int ScrollTop { get; set; }

        /// <summary>
        /// Specifies the visual scrolling region bottom in base 0. -1 signifies no bottom set.
        /// </summary>
        /// <remarks>
        /// When there is no bottom set and scrolling passes the bottom of the screen, then the history
        /// buffer is advanced. But if the scrolling region is configured to be a portion of the screen
        /// the history buffer is simply adjusted (overwritten) in place.
        /// </remarks>
        public int ScrollBottom { get; set; } = -1;

        /// <summary>
        /// Specifies the left margin in base 0.
        /// </summary>
        public int LeftMargin { get; set; }

        /// <summary>
        /// Specifies the right margin in base 0. -1 specifies no right margin is currently set.
        /// </summary>
        public int RightMargin { get; set; } = -1;

        /// <summary>
        /// Configures that left and right margins should be used
        /// </summary>
        public bool LeftAndRightMarginEnabled { get; set; }

        /// <summary>
        /// Holds a reference to the last character set.
        /// </summary>
        /// <remarks>
        /// This is used for repeating characters as per (Repeat the preceding graphic character Ps times (REP).)
        /// however I'm not convinced it will always used wholesomely. 
        /// </remarks>
        private TerminalCharacter LastCharacter { get; set; }

        /// <summary>
        /// Holds the last transmitted mouse position and should be -1,-1 when it is forgotten
        /// </summary>
        public TextPosition LastMousePosition = new TextPosition(-1, -1);

        /// <summary>
        /// Returns true if mouse tracking is enabled
        /// </summary>
        public bool MouseTrackingEnabled
        {
            get
            {
                return
                    UrxvtMouseMode |
                    UseAllMouseTracking |
                    CellMotionMouseTracking |
                    HighlightMouseTracking |
                    X10SendMouseXYOnButton |
                    X11SendMouseXYOnButton |
                    SgrMouseMode;
            }
        }

        public string WindowTitle { get; private set; }

        /// <summary>
        /// Provides a dump of the current state of this control.
        /// </summary>
        /// <todo>
        /// This is in desperate need of updating.
        /// </todo>
        public string DebugText
        {
            get
            {
                return
                    "TopRow: " + TopRow.ToString() + "\n" +
                    "Columns: " + Columns.ToString() + "\n" +
                    "Rows: " + Rows.ToString() + "\n" +
                    "VisibleColumns: " + VisibleColumns.ToString() + "\n" +
                    "VisibleRows: " + VisibleRows.ToString() + "\n" +
                    "HighlightMouseTracking: " + HighlightMouseTracking.ToString() + "\n" +
                    "CellMotionMouseTracking: " + CellMotionMouseTracking.ToString() + "\n" +
                    "SgrMouseMode: " + SgrMouseMode.ToString() + "\n" +
                    "CursorState: " + "\n" + CursorState.ToString()
                    ;
            }
        }

        /// <summary>
        /// Basic constructor
        /// </summary>
        public VirtualTerminalController()
        {
            Buffer = normalBuffer;
            ViewPort = new VirtualTerminalViewPort(this);
        }

        /// <summary>
        /// Called to transmit data from this control.
        /// </summary>
        public EventHandler<SendDataEventArgs> SendData;

        /// <summary>
        /// Emitted when the server sends a new window title
        /// </summary>
        public EventHandler<TextEventArgs> WindowTitleChanged;

        /// <summary>
        /// Emitted when the terminal is configured to be a new size
        /// </summary>
        public EventHandler<SizeEventArgs> SizeChanged;

        /// <summary>
        /// Enables storing of raw text for scripting tools
        /// </summary>
        public bool StoreRawText { get; set; }

        private char[] _rawText;
        private int _rawTextLength;

        /// <summary>
        /// Queued raw text data
        /// </summary>
        public char [] RawText
        {
            get
            {
                if (_rawTextLength == 0)
                    return new char[] { };

                var result = _rawText.Take(_rawTextLength).ToArray();
                _rawTextLength = 0;
                return result;
            }
        }

        /// <summary>
        /// Emits events when log items are generated by this control
        /// </summary>
        public EventHandler<TextEventArgs> OnLog;

        public int ChangeCount { get; private set; }

        public VirtualTerminalViewPort ViewPort { get; private set; }

        private ECharacterSet CharacterSet
        {
            get
            {
                switch (CursorState.CharacterSetMode)
                {
                    case ECharacterSetMode.IsoG1:
                        return CursorState.G1;
                    case ECharacterSetMode.IsoG2:
                        return CursorState.G2;
                    case ECharacterSetMode.IsoG3:
                        return CursorState.G3;
                    case ECharacterSetMode.Vt300G1:
                        return CursorState.Vt300G1;
                    case ECharacterSetMode.Vt300G2:
                        return CursorState.Vt300G2;
                    case ECharacterSetMode.Vt300G3:
                        return CursorState.Vt300G3;
                    default:
                        return CursorState.G0;
                }
            }
        }

        

        private ECharacterSet RightCharacterSet
        {
            get
            {
                switch (CursorState.CharacterSetModeR)
                {
                    case ECharacterSetMode.IsoG1:
                        return CursorState.G1;
                    case ECharacterSetMode.IsoG2:
                        return CursorState.G2;
                    case ECharacterSetMode.IsoG3:
                        return CursorState.G3;
                    default:
                        return CursorState.G0;
                }
            }
        }

        /// <summary>
        /// Returns whether the input stream should be processed as Utf8 or raw bytes
        /// </summary>
        /// <returns>Whether to process data received as Utf8</returns>
        public bool IsUtf8()
        {
            return CursorState.Utf8;
        }

        /// <summary>
        /// Resets the change counter.
        /// </summary>
        /// <remarks>
        /// This is scheduled for removal as soon as invalidate is properly implemented
        /// </remarks>
        public void ClearChanges()
        {
            ChangeCount = 0;
        }

        /// <summary>
        /// Specifies whether there have been any changes to the output since the last ClearChanges() call
        /// </summary>
        public bool Changed { get { return ChangeCount > 0; } }

        /// <summary>
        /// Returns the character at the given screen location
        /// </summary>
        /// <param name="x">The column in base-0 coordinates</param>
        /// <param name="y">The row in base 0 coordinates</param>
        /// <returns>The character or null if none present</returns>
        internal TerminalCharacter GetVisibleCharModel(int x, int y)
        {
            if ((TopRow + y) >= Buffer.Count)
                return null;

            var line = Buffer[TopRow + y];
            if (line.Count <= x)
                return null;

            return line[x];
        }

        /// <summary>
        /// Returns a character at the given visible screen position
        /// </summary>
        /// <param name="x">The column in base-0 coordinates</param>
        /// <param name="y">The row in base 0 coordinates</param>
        /// <returns>The character or space if none present</returns>
        internal string GetVisibleChar(int x, int y)
        {
            if ((TopRow + y) >= Buffer.Count)
                return " ";

            var line = Buffer[TopRow + y];
            if (line.Count <= x)
                return " ";

            return line[x].Char.ToString() + line[x].CombiningCharacters;
        }

        /// <summary>
        /// Returns a span of characters on a line referenced relative to the top visible line
        /// </summary>
        /// <remarks>
        /// This is meant primarily for unit testing
        /// </remarks>
        /// <param name="x">Base-0 index of the first column</param>
        /// <param name="y">Base-0 index of the row</param>
        /// <param name="count">The number of characters to return</param>
        /// <returns></returns>
        internal string GetVisibleChars(int x, int y, int count)
        {
            string result = "";

            for (var i = 0; i < count; i++)
                result += GetVisibleChar(x + i, y);

            return result;
        }

        /// <summary>
        /// Returns the visible text on the screen as per TopRow and the logical rows and columns
        /// </summary>
        /// <returns>The screen text with each line separated by a line feed</returns>
        internal string ScreenText
        {
            get
            {
                string result = "";

                for (var y = 0; y < Rows; y++)
                {
                    for (var x = 0; x < Columns; x++)
                        result += GetVisibleChar(x, y);

                    if (y < (Rows - 1))
                        result += '\n';
                }

                return result;
            }
        }

        /// <summary>
        /// Returns the visible text on the screen as per TopRow and the logical rows and columns
        /// </summary>
        /// <returns>The screen text with each line separated by a line feed</returns>
        internal string GetScreenText()
        {
            string result = "";

            for (var y = 0; y < Rows; y++)
            {
                for (var x = 0; x < Columns; x++)
                    result += GetVisibleChar(x, y);

                if (y < (Rows - 1))
                    result += '\n';
            }

            return result;
        }

        /// <summary>
        /// Makes a map of what the screen protection looks like
        /// </summary>
        internal string ProtectionMap
        {
            get
            {
                string result = "";

                for (var y = 0; y < Rows; y++)
                {
                    for (var x = 0; x < Columns; x++)
                    {
                        var ch = GetVisibleCharModel(x, y);
                        result += (ch != null && (ch.Attributes.Protected == 1 || (GuardedArea != null && GuardedArea.Contains(x, y)))) ? "X" : ".";
                    }

                    if (y < (Rows - 1))
                        result += '\n';
                }

                return result;
            }
        }

        /// <summary>
        /// Returns a visible structure of the screen organized as rows and spans.
        /// </summary>
        /// <param name="startingLine">The zero based line to return relative to the history buffer</param>
        /// <param name="lineCount">The number of lines to return. -1 returns everything from the start of the buffer</param>
        /// <param name="width">The fixed width of the screen. If this is less than 1, then no right padding will be applied</param>
        /// <param name="invertedRange">Specifies the range to invert. This is so that text selection can be handled.</param>
        /// <returns>A list of rows and spans for painting</returns>
        public List<Layout.LayoutRow> GetPageSpans(int startingLine, int lineCount, int width=-1, TextRange invertedRange=null)
        {
            var result = new List<Layout.LayoutRow>();

            if (invertedRange == null)
                invertedRange = 
                    new TextRange
                    {
                        Start = new TextPosition
                        {
                            Row = -1
                        },
                        End = new TextPosition
                        {
                            Row = -1
                        }
                    };

            var currentAttribute = new TerminalAttribute();

            if (lineCount == -1)
                lineCount = Buffer.Count - startingLine;

            for(var y=0; y<lineCount; y++)
            {
                var sourceLine = GetLine(y + startingLine);
                var sourceChar = (sourceLine == null || sourceLine.Count == 0) ? null : sourceLine[0];

                currentAttribute = sourceChar == null ? NullAttribute : ((CursorState.ReverseVideoMode ^ invertedRange.Contains(0, y+ startingLine) ^ sourceChar.Attributes.Reverse) ? sourceChar.Attributes.Inverse : sourceChar.Attributes);

                var currentRow = new Layout.LayoutRow
                {
                    LogicalRowNumber = y + startingLine,
                    DoubleWidth = (sourceLine == null) ? false : sourceLine.DoubleWidth,
                    DoubleHeightTop = (sourceLine == null) ? false : sourceLine.DoubleHeightTop,
                    DoubleHeightBottom = (sourceLine == null) ? false : sourceLine.DoubleHeightBottom
                };
                result.Add(currentRow);

                var currentSpan = new Layout.LayoutSpan
                {
                    ForgroundColor = currentAttribute.WebColor,
                    BackgroundColor = currentAttribute.BackgroundWebColor,
                    Hidden = currentAttribute.Hidden,
                    Blink = currentAttribute.Blink,
                    Bold = currentAttribute.Bright,
                    Italic = false,
                    Underline = currentAttribute.Underscore,
                    Text = ""
                };
                currentRow.Spans.Add(currentSpan);

                if (sourceLine == null && width > 0)
                {
                    currentSpan.Text = string.Empty.PadRight(width, ' ');
                    currentSpan.ForgroundColor = CursorState.ReverseVideoMode ? NullAttribute.BackgroundWebColor : NullAttribute.WebColor;
                    currentSpan.BackgroundColor = CursorState.ReverseVideoMode ? NullAttribute.WebColor : NullAttribute.BackgroundWebColor;
                }
                else if (sourceLine != null)
                {
                    var lineWidth = width > 0 ? width : sourceLine.Count;
                    if (sourceLine.DoubleWidth)
                        lineWidth /= 2;

                    var x = 0;
                    while (x < lineWidth && x < sourceLine.Count)
                    {
                        var attributeAtThisPosition = ((CursorState.ReverseVideoMode ^ invertedRange.Contains(x, y + startingLine) ^ sourceLine[x].Attributes.Reverse) ? sourceLine[x].Attributes.Inverse : sourceLine[x].Attributes);
                        if (!currentAttribute.Equals(attributeAtThisPosition))
                        {
                            currentAttribute = attributeAtThisPosition;

                            currentSpan = new Layout.LayoutSpan
                            {
                                ForgroundColor = currentAttribute.WebColor,
                                BackgroundColor = currentAttribute.BackgroundWebColor,
                                Hidden = currentAttribute.Hidden,
                                Blink = currentAttribute.Blink,
                                Bold = currentAttribute.Bright,
                                Italic = false,
                                Underline = currentAttribute.Underscore,
                                Text = ""
                            };
                            currentRow.Spans.Add(currentSpan);
                        }

                        currentSpan.Text += sourceLine[x].Char + sourceLine[x].CombiningCharacters;
                        x++;
                    }

                    if (x < lineWidth)
                    {
                        currentSpan = new Layout.LayoutSpan
                        {
                            ForgroundColor = CursorState.ReverseVideoMode ? NullAttribute.BackgroundWebColor : NullAttribute.WebColor,
                            BackgroundColor = CursorState.ReverseVideoMode ? NullAttribute.WebColor : NullAttribute.BackgroundWebColor,
                            Hidden = NullAttribute.Hidden,
                            Blink = NullAttribute.Blink,
                            Bold = NullAttribute.Bright,
                            Italic = false,
                            Underline = NullAttribute.Underscore,
                            Text = string.Empty.PadRight(lineWidth - x, ' ')
                        };
                        currentRow.Spans.Add(currentSpan);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Assembles a string which represents the page as a series of spans with attributes for testing
        /// </summary>
        internal string PageAsSpans
        {
            get
            {
                var currentAttribute = new TerminalAttribute();

                string result = "";

                for (var y = 0; y < Rows; y++)
                {
                    for (var x = 0; x < Columns; x++)
                    {
                        var ch = GetVisibleCharModel(x, y);

                        if (ch == null)
                        {
                            result += "→";
                        }
                        else
                        {
                            if (!currentAttribute.Equals(ch.Attributes))
                            {
                                if (currentAttribute.Blink != ch.Attributes.Blink)
                                    result += ch.Attributes.Blink ? "<blink>" : "</blink>";

                                if (currentAttribute.Bright != ch.Attributes.Bright)
                                    result += ch.Attributes.Bright ? "<bright>" : "</bright>";

                                if (currentAttribute.Hidden != ch.Attributes.Hidden)
                                    result += ch.Attributes.Hidden ? "<hidden>" : "</hidden>";

                                if (currentAttribute.Protected != ch.Attributes.Protected)
                                    result += "<protected mode='" + ch.Attributes.Protected + "' />";

                                if (currentAttribute.Reverse != ch.Attributes.Reverse)
                                    result += ch.Attributes.Reverse ? "<reverse>" : "</reverse>";

                                if (currentAttribute.Standout != ch.Attributes.Standout)
                                    result += ch.Attributes.Blink ? "<standout>" : "</standout>";

                                if (currentAttribute.Underscore != ch.Attributes.Underscore)
                                    result += ch.Attributes.Underscore ? "<underscore>" : "</underscore>";

                                if (currentAttribute.WebColor != ch.Attributes.WebColor)
                                    result += "<foreground value='" + ch.Attributes.WebColor + "' />";

                                if (currentAttribute.BackgroundWebColor != ch.Attributes.BackgroundWebColor)
                                    result += "<background value='" + ch.Attributes.BackgroundWebColor + "' />";

                                currentAttribute = ch.Attributes.Clone();
                            }

                            result += ch.Char + ((ch.CombiningCharacters == null) ? "" : ch.CombiningCharacters);
                        }
                    }

                    if (y < (Rows - 1))
                        result += "↵";
                }

                return result;
            }
        }

        /// <summary>
        /// Returns the text specified by the provided range
        /// </summary>
        /// <remarks>
        /// This is not rectangle based but stream based
        /// TODO: Consider moving to viewport
        /// </remarks>
        /// <param name="range">The range to return the text from</param>
        /// <returns>The requested text if present</returns>
        public string GetText(TextRange range)
        {
            return GetText(range.Start.Column, range.Start.Row, range.End.Column, range.End.Row);
        }

        /// <summary>
        /// Returns the text 
        /// </summary>
        /// <remarks>
        /// This is not rectangle based but stream based
        /// TODO: Consider moving to viewport
        /// </remarks>
        /// <param name="startColumn">Starting column</param>
        /// <param name="startRow">Starting row</param>
        /// <param name="endColumn">End column</param>
        /// <param name="endRow">End row</param>
        /// <returns>The requested text if present</returns>
        public string GetText(int startColumn, int startRow, int endColumn, int endRow)
        {
            if (startRow > endRow || (startRow == endRow && startColumn > endColumn))
            {
                var holder = startColumn;
                startColumn = endColumn;
                endColumn = holder;

                holder = startRow;
                startRow = endRow;
                endRow = holder;
            }

            string result = "";

            if (startRow >= Buffer.Count)
                return result;

            var line = GetLine(startRow);

            if (startRow == endRow)
            {
                for (int i = startColumn; line != null && i <= endColumn && i < line.Count; i++)
                    result += line[i].Char;

                return result;
            }

            for (int i = startColumn; line != null && i < line.Count; i++)
                result += line[i].Char;

            for (int y = startRow + 1; y < endRow; y++)
            {
                result += '\n';

                line = GetLine(y);
                for (int i = 0; line != null && i < line.Count; i++)
                    result += line[i].Char;
            }

            result += '\n';

            line = GetLine(endRow);
            for (int i = 0; line != null && i <= endColumn && i < line.Count; i++)
                result += line[i].Char;

            return result;
        }

        /// <summary>
        /// Called to perform a full reset of the buffers.
        /// </summary>
        /// <remark>
        /// This call is meant for use by the stream handler and therefore doesn't delete buffers but
        /// simply scrolls them past the last viewable point.
        /// </remark>
        public void FullReset()
        {
            alternativeBufferTopRow = alternativeBuffer.Count;
            normalBufferTopRow = normalBuffer.Count;
            TopRow = normalBufferTopRow;
            Vt52Mode = false;

            ActiveBuffer = EActiveBuffer.Normal;

            SavedCursorState = null;
            CursorState = new TerminalCursorState();
            NullAttribute = new TerminalAttribute();

            ScrollTop = 0;
            ScrollBottom = -1;
            LeftMargin = 0;
            RightMargin = -1;
            LeftAndRightMarginEnabled = false;

            LastCharacter = null;

            Columns = VisibleColumns;
            Rows = VisibleRows;

            BracketedPasteMode = false;
            GuardedArea = null;
            GuardedAreaTransferMode = false;
            ErasureMode = false;
            ReverseWrapAroundMode = false;
            SmoothScrollMode = false;

            LastMousePosition.Set(-1, -1);

            ChangeCount++;
        }

        private void Log(string message)
        {
            if (Debugging)
            {
                //System.Diagnostics.Debug.WriteLine("Terminal: " + message);
                OnLog?.Invoke(this, new TextEventArgs { Text = "Terminal: " + message });
            }
        }

        private void LogController(string message)
        {
            if (Debugging)
            {
                System.Diagnostics.Debug.WriteLine("Controller: " + message);
                OnLog?.Invoke(this, new TextEventArgs { Text = "Controller: " + message });
            }
        }

        private void LogExtreme(string message)
        {
            if (Debugging)
            {
                //System.Diagnostics.Debug.WriteLine("Terminal: (c=" + CursorState.CurrentColumn.ToString() + ",r=" + CursorState.CurrentRow.ToString() + ")" + message);
                OnLog?.Invoke(this, new TextEventArgs { Text = "Terminal: (c = " + CursorState.CurrentColumn.ToString() + ", r = " + CursorState.CurrentRow.ToString() + ")" + message });
            }
        }

        public void EnableNationalReplacementCharacterSets(bool enable)
        {
            LogController($"EnableNationalReplacementCharacterSets(enable:{enable})");
            CursorState.NationalCharacterReplacementMode = enable;
        }

        public void SetCharacterSet(ECharacterSet characterSet, ECharacterSetMode mode)
        {
            LogController("SetCharacterSet(characterSet:" + characterSet.ToString() + ")");

            switch (mode)
            {
                case ECharacterSetMode.IsoG0:
                    CursorState.G0 = characterSet;
                    break;
                case ECharacterSetMode.IsoG1:
                    CursorState.G1 = characterSet;
                    break;
                case ECharacterSetMode.IsoG2:
                    CursorState.G2 = characterSet;
                    break;
                case ECharacterSetMode.IsoG3:
                    CursorState.G3 = characterSet;
                    break;
                case ECharacterSetMode.Vt300G1:
                    CursorState.G1 = characterSet;
                    CursorState.Vt300G1 = characterSet;
                    break;
                case ECharacterSetMode.Vt300G2:
                    CursorState.Vt300G2 = characterSet;
                    CursorState.G2 = characterSet;
                    break;
                case ECharacterSetMode.Vt300G3:
                    CursorState.Vt300G3 = characterSet;
                    CursorState.G3 = characterSet;
                    break;
            }
        }

        public void TabSet()
        {
            var stop = CursorState.CurrentColumn;
            LogController("TabSet() [cursorX=" + stop.ToString() + "]");

            var tabStops = CursorState.TabStops;
            int index = 0;
            while (index < tabStops.Count && tabStops[index] < stop)
                index++;

            if (index >= tabStops.Count)
                tabStops.Add(stop);
            else if (tabStops[index] != stop)
                tabStops.Insert(index, stop);
        }

        public void Tab()
        {
            var current = CursorState.CurrentColumn;
            LogController("Tab() [cursorX=" + current.ToString() + "]");

            if (StoreRawText)
            {
                if (_rawText == null)
                    _rawText = new char[1024];
                else if ((_rawTextLength + 1) >= _rawText.Length)
                    Array.Resize(ref _rawText, _rawText.Length * 2);

                _rawText[_rawTextLength++] = '\t';
            }

            var tabStops = CursorState.TabStops;
            int index = 0;
            while (index < tabStops.Count && tabStops[index] <= current)
                index++;

            if (index < tabStops.Count)
                SetCursorPosition(tabStops[index] + 1, CursorState.CurrentRow + 1);

            if (CursorState.WordWrap && CursorState.CurrentColumn >= CurrentLineColumns)
                CursorState.CurrentColumn = CurrentLineColumns - 1;
        }

        public void ReverseTab()
        {
            var current = CursorState.CurrentColumn;
            LogController("ReverseTab() [cursorX=" + current.ToString() + "]");

            var tabStops = CursorState.TabStops;
            int index = tabStops.Count - 1;
            while (index >= 0 && tabStops[index] >= current)
                index--;

            if (index >= 0)
                SetCursorPosition(tabStops[index] + 1, CursorState.CurrentRow + 1);
        }

        public void ClearTabs()
        {
            LogController("ClearTabs()");

            CursorState.TabStops.Clear();
        }

        public void ClearTab()
        {
            var stop = CursorState.CurrentColumn;

            LogController("ClearTab() [cursorX=" + stop.ToString() + "]");

            var tabStops = CursorState.TabStops;
            int index = 0;
            while (index < tabStops.Count && tabStops[index] < stop)
                index++;

            if (index < tabStops.Count && tabStops[index] == stop)
                tabStops.RemoveAt(index);
        }

        public void CarriageReturn()
        {
            LogExtreme("Carriage return");

            CursorState.CurrentColumn = 0;
            ChangeCount++;
        }

        private void FillVisualRect(int x1, int y1, int x2, int y2, char ch, TerminalAttribute attr)
        {
            LogController("FillVisualRect(x1:" + x1.ToString() + ",y1:" + y1.ToString() + ",x2:" + x2.ToString() + ",y2:" + y2.ToString() + ")");

            for (var y = y1; y <= y2; y++)
                for (var x = x1; x <= x2; x++)
                    SetCharacter(x, y, ch, attr);
        }

        private TerminalLine GetCurrentLine()
        {
            return GetLine(TopRow + CursorState.CurrentRow);
        }

        //private TerminalCharacter GetCurrentCharacter()
        //{
        //    var line = GetCurrentLine();
        //    if (line == null || line.Count <= CursorState.CurrentColumn)
        //        return null;

        //    return line[CursorState.CurrentColumn];
        //}

        private TerminalCharacter GetCharacterAt(int row, int column)
        {
            var line = GetVisualLine(row);
            if (line == null || line.Count <= column)
                return null;

            return line[column];
        }

        public bool IsProtected(int row, int column)
        {
            var character = GetCharacterAt(row, column);
            return character == null ? false : (character.Attributes.Protected == 1);
        }

        /// <summary>
        /// Returns the specified line within the buffer or null if past end
        /// </summary>
        /// <param name="lineNumber">The line number</param>
        /// <returns>The line requested or null</returns>
        private TerminalLine GetLine(int lineNumber)
        {
            if (lineNumber >= Buffer.Count)
                return null;

            return Buffer[lineNumber];
        }

        private TerminalLine GetVisualLine(int y)
        {
            return GetLine(y + TopRow);
        }

        /// <summary>
        /// Returns the character at the given position or a new blank character if none is present
        /// </summary>
        /// <param name="x">The column in base 0</param>
        /// <param name="y">The row in base zero relative to the full history buffer</param>
        /// <returns></returns>
        private TerminalCharacter GetCharacter(int x, int y)
        {
            var line = GetVisualLine(y);

            if (line == null || x >= line.Count)
            {
                return new TerminalCharacter
                {
                    Char = ' ',
                    Attributes = NullAttribute.Clone()
                };
            }

            return line[x];
        }

        /// <summary>
        /// Copies a vertical span of characters from one line to another
        /// </summary>
        /// <param name="x1">The 0-based left column</param>
        /// <param name="x2">The 0-based right column</param>
        /// <param name="fromLine">The 0-based source row relative to the buffer</param>
        /// <param name="toLine">The 0-based destination row relative to the buffer</param>
        private void MoveCharacters(int x1, int x2, int fromLine, int toLine)
        {
            for (int x = x1; x <= x2; x++)
            {
                var ch = GetCharacter(x, fromLine);
                SetCharacter(x, toLine, ch.Char, ch.Attributes);
            }
        }

        /// <summary>
        /// Scrolls the contents of the buffer vertically by the given number of rows
        /// </summary>
        /// <param name="x1">The zero based left column</param>
        /// <param name="y1">The zero based top row relative to the active area</param>
        /// <param name="x2">The zero based right column</param>
        /// <param name="y2">The zero based bottom row relative to the active area</param>
        /// <param name="count">The number of rows to scroll. Positive scrolls up, negative scrolls down</param>
        private void ScrollVisualRect(int x1, int y1, int x2, int y2, int count)
        {
            LogController("ScrollVisualRect(x1:" + x1.ToString() + ",y1:" + y1.ToString() + ",x2:" + x2.ToString() + ",y2:" + y2.ToString() + ",count:" + count.ToString() + ")");

            if (count == 0)
                return;

            int height = y2 - y1 + 1;
            if (Math.Abs(count) >= height)
            {
                FillVisualRect(x1, y1, x2, y2, ' ', CursorState.Attributes);
                return;
            }

            if (count > 0)
            {
                for (var i = 0; i < height - count; i++)
                    MoveCharacters(x1, x2, y1 + i + count, y1 + i);

                FillVisualRect(x1, y1 + height - count, x2, y2, ' ', CursorState.Attributes);
            }
            else
            {
                count = Math.Abs(count);
                for (var i = 0; i < height - count; i++)
                    MoveCharacters(x1, x2, y2 - i - count, y2 - i);

                FillVisualRect(x1, y1, x2, y1 + count - 1, ' ', CursorState.Attributes);
            }
        }

        /// <summary>
        /// Scrolls the current buffer by the given number of rows
        /// </summary>
        /// <remarks>
        /// This function takes into consideration the margins (for left and right as well as top and bottom)
        /// </remarks>
        /// <param name="rows"></param>
        public void Scroll(int rows)
        {
            if (LeftAndRightMarginEnabled && CursorState.CurrentColumn >= LeftMargin && CursorState.CurrentColumn <= RightMargin)
            {
                ScrollVisualRect(
                    LeftMargin,
                    ScrollTop,
                    RightMargin,
                    ScrollBottom == -1 ? Rows - 1 : ScrollBottom,
                    rows
                );
            }
            else
            {
                ScrollVisualRect(
                   0,
                   ScrollTop,
                   VisibleColumns - 1,
                   ScrollBottom == -1 ? Rows - 1 : ScrollBottom,
                   rows
               );
            }
        }

        /// <summary>
        /// Copies a vertical span of characters from one line to another
        /// </summary>
        /// <param name="y1">The 0-based top row</param>
        /// <param name="y2">The 0-based bottom row</param>
        /// <param name="fromColumn">The 0-based source column relative to the buffer</param>
        /// <param name="toColumn">The 0-based destination column relative to the buffer</param>
        private void MoveCharactersAcross(int y1, int y2, int fromColumn, int toColumn)
        {
            for (int y = y1; y <= y2; y++)
            {
                var ch = GetCharacter(fromColumn, y);
                SetCharacter(toColumn, y, ch.Char, ch.Attributes);
            }
        }

        /// <summary>
        /// Scrolls the contents of the buffer horizontally by the given number of columns
        /// </summary>
        /// <param name="x1">The zero based left column</param>
        /// <param name="y1">The zero based top row relative to the active area</param>
        /// <param name="x2">The zero based right column</param>
        /// <param name="y2">The zero based bottom row relative to the active area</param>
        /// <param name="count">The number of columns to scroll. Positive scrolls right, negative scrolls left</param>
        private void ScrollVisualRectAcross(int x1, int y1, int x2, int y2, int count)
        {
            LogController("ScrollVisualRectAcross(x1:" + x1.ToString() + ",y1:" + y1.ToString() + ",x2:" + x2.ToString() + ",y2:" + y2.ToString() + ",count:" + count.ToString() + ")");

            if (count == 0)
                return;

            int width = x2 - x1 + 1;
            if (Math.Abs(count) >= width)
            {
                FillVisualRect(x1, y1, x2, y2, ' ', CursorState.Attributes);
                return;
            }

            if (count > 0)
            {
                for (var i = 0; i < width - count; i++)
                    MoveCharactersAcross(y1, y2, x1 + i + count, x1 + i);

                FillVisualRect(x1 + width - count, y1, x2, y2, ' ', CursorState.Attributes);
            }
            else
            {
                count = Math.Abs(count);
                for (var i = 0; i < width - count; i++)
                    MoveCharactersAcross(y1, y2, x2 - i - count, x2 - i);

                FillVisualRect(x1, y1, x1 + count - 1, y2, ' ', CursorState.Attributes);
            }
        }

        /// <summary>
        /// Scrolls the buffer the given number of columns within the margins
        /// </summary>
        /// <param name="columns">The number of columns to scroll</param>
        public void ScrollAcross(int columns)
        {
            if (LeftAndRightMarginEnabled && CursorState.CurrentColumn >= LeftMargin && CursorState.CurrentColumn <= RightMargin)
            {
                ScrollVisualRectAcross(
                    LeftMargin,
                    ScrollTop,
                    RightMargin,
                    ScrollBottom == -1 ? Rows - 1 : ScrollBottom,
                    columns
                );
            }
            else
            {
                ScrollVisualRectAcross(
                   0,
                   ScrollTop,
                   VisibleColumns - 1,
                   ScrollBottom == -1 ? Rows - 1 : ScrollBottom,
                   columns
               );
            }
        }

        public void NewLine()
        {
            LogExtreme("NewLine()");

            if (StoreRawText)
            {
                if (_rawText == null)
                    _rawText = new char[1024];
                else if ((_rawTextLength + 1) >= _rawText.Length)
                    Array.Resize(ref _rawText, _rawText.Length * 2);

                _rawText[_rawTextLength++] = '\n';
            }

            if (LeftAndRightMarginEnabled && CursorState.CurrentColumn >= LeftMargin && CursorState.CurrentColumn <= RightMargin)
            {
                CursorState.CurrentRow++;
                if (
                    (ScrollBottom == -1 && CursorState.CurrentRow >= VisibleRows) ||
                    (ScrollBottom >= 0 && CursorState.CurrentRow == ScrollBottom + 1)
                )
                {
                    ScrollVisualRect(
                        LeftMargin,
                        ScrollTop,
                        RightMargin,
                        ScrollBottom == -1 ? Rows - 1 : ScrollBottom,
                        1
                    );
                    CursorState.CurrentRow--;
                }
            }
            else
            {
                CursorState.CurrentRow++;

                if (ScrollBottom == -1 && CursorState.CurrentRow >= VisibleRows)
                {
                    LogController("Scroll all (before:" + TopRow.ToString() + ",after:" + (TopRow + 1).ToString() + ")");
                    TopRow++;
                    CursorState.CurrentRow--;

                    while (TopRow > MaximumHistoryLines)
                    {
                        Buffer.RemoveAt(0);
                        TopRow--;
                    }
                }
                else if (ScrollBottom >= 0 && CursorState.CurrentRow == ScrollBottom + 1)
                {
                    LogController("Scroll region");

                    if (Buffer.Count > (ScrollBottom + TopRow))
                        Buffer.Insert(ScrollBottom + TopRow + 1, new TerminalLine());

                    Buffer.RemoveAt(ScrollTop + TopRow);

                    CursorState.CurrentRow--;
                }
                else if (CursorState.CurrentRow >= VisibleRows)
                    CursorState.CurrentRow--;

                ChangeCount++;
            }

            if (CursorState.AutomaticNewLine)
                CarriageReturn();
        }

        /// <summary>
        /// Moves the cursor to the next vertical tab stop
        /// </summary>
        /// <todo>
        /// This is not completely implemented as vertical tab stops are not implemented.
        /// Instead this code makes the assumption that all lines are vertical tab stops.
        /// </todo>
        public void VerticalTab()
        {
            LogController("VerticalTab()");
            MoveCursorRelative(0, 1);
        }

        /// <summary>
        /// Moves the cursor down one line similar to new line.
        /// </summary>
        /// <remarks>
        /// I don't see anything more definitive for how to process form feed on a screen
        /// </remarks>
        public void FormFeed()
        {
            LogController("FormFeed()");
            MoveCursorRelative(0, 1);
        }

        public void ReverseIndex()
        {
            LogController("ReverseIndex()");

            if (
                LeftAndRightMarginEnabled &&
                CursorState.CurrentColumn >= LeftMargin &&
                CursorState.CurrentColumn <= RightMargin
            )
            {
                CursorState.CurrentRow--;
                if (CursorState.CurrentRow == (ScrollTop - 1))
                {
                    var scrollBottom = 0;
                    if (ScrollBottom == -1)
                        scrollBottom = TopRow + VisibleRows - 1;
                    else
                        scrollBottom = ScrollBottom;

                    ScrollVisualRect(
                        LeftMargin,
                        ScrollTop,
                        RightMargin,
                        scrollBottom,
                        -1
                    );
                    CursorState.CurrentRow++;
                }
            }
            else
            {
                CursorState.CurrentRow--;

                if (CursorState.CurrentRow == (ScrollTop - 1))
                {
                    var scrollBottom = 0;
                    if (ScrollBottom == -1)
                        scrollBottom = TopRow + VisibleRows - 1;
                    else
                        scrollBottom = TopRow + ScrollBottom;

                    if (Buffer.Count > scrollBottom)
                        Buffer.RemoveAt(scrollBottom);

                    Buffer.Insert(TopRow + ScrollTop, new TerminalLine());

                    CursorState.CurrentRow++;
                }
            }
        }

        /// <summary>
        /// Returns the number of columns for the current line for processing tabs and mouse motion
        /// </summary>
        /// <remarks>
        /// This should consider character widths. At this time, it does not support anything other than normal and
        /// double width.
        /// </remarks>
        private int CurrentLineColumns
        {
            get
            {
                var line = GetCurrentLine();
                if (line == null)
                    return Columns;

                return (line.DoubleWidth | line.DoubleHeightTop | line.DoubleHeightBottom) ? (Columns >> 1) : Columns;
            }
        }

        public void Backspace()
        {
            LogExtreme("Backspace");

            if (CursorState.CurrentColumn > 0)
            {
                if (CursorState.WordWrap && CursorState.CurrentColumn >= CurrentLineColumns)
                    CursorState.CurrentColumn = CurrentLineColumns - 1;

                CursorState.CurrentColumn--;

                ChangeCount++;
            }
        }

        public void Bell()
        {
            LogExtreme("Unimplemented: Bell()");
        }

        public void MoveCursorRelative(int x, int y)
        {
            LogController("MoveCursorRelative(x:" + x.ToString() + ",y:" + y.ToString() + ",vis:[" + VisibleColumns.ToString() + "," + VisibleRows.ToString() + "]" + ")");

            CursorState.CurrentRow += y;
            if (CursorState.CurrentRow < ScrollTop)
                CursorState.CurrentRow = ScrollTop;

            var scrollBottom = (ScrollBottom == -1) ? Rows - 1 : ScrollBottom;
            if (CursorState.CurrentRow > scrollBottom)
                CursorState.CurrentRow = scrollBottom;

            CursorState.CurrentColumn += x;
            if (CursorState.CurrentColumn < 0)
                CursorState.CurrentColumn = 0;
            if (CursorState.CurrentColumn >= CurrentLineColumns)
                CursorState.CurrentColumn = CurrentLineColumns - 1;

            ChangeCount++;
        }

        public void SetCursorPosition(int column, int row)
        {
            LogController("SetCursorPosition(column:" + column.ToString() + ",row:" + row.ToString() + ")");

            CursorState.CurrentRow = row - 1 + (CursorState.OriginMode ? ScrollTop : 0);
            if (CursorState.CurrentRow < 0)
                CursorState.CurrentRow = 0;

            if (CursorState.OriginMode && ScrollBottom > -1 && CursorState.CurrentRow > ScrollBottom)
                CursorState.CurrentRow = ScrollBottom;
            else if (ScrollBottom == -1 && CursorState.CurrentRow >= VisibleRows)
                CursorState.CurrentRow = VisibleRows - 1;

            CursorState.CurrentColumn = column - 1;
            if (LeftAndRightMarginEnabled)
            {
                if (CursorState.OriginMode && CursorState.CurrentColumn < LeftMargin)
                    CursorState.CurrentColumn = LeftMargin;
                if (CursorState.CurrentColumn >= RightMargin)
                    RightMargin = RightMargin;
            }

            if (CursorState.WordWrap && CursorState.CurrentColumn > CurrentLineColumns)
                CursorState.CurrentColumn = CurrentLineColumns;
            else if(!CursorState.WordWrap && CursorState.CurrentColumn >= CurrentLineColumns)
                CursorState.CurrentColumn = CurrentLineColumns - 1;

            ChangeCount++;
        }

        public void SetAbsoluteRow(int row)
        {
            SetCursorPosition(CursorState.CurrentColumn + 1, row);
        }

        public void InsertBlanks(int count)
        {
            LogController("InsertBlank(count:" + count.ToString() + ")");

            InsertBlanks(count, TopRow + CursorState.CurrentRow);

            ChangeCount++;
        }

        public void EraseCharacter(int count)
        {
            LogController("EraseCharacter(count:" + count.ToString() + ")");

            for (var i = 0; i < count; i++)
                SetCharacter(CursorState.CurrentColumn + i, CursorState.CurrentRow, ' ', CursorState.Attributes);
        }

        public void DeleteCharacter(int count)
        {
            LogController("DeleteCharacter(count:" + count.ToString() + ")");

            DeleteCharacter(count, CursorState.CurrentRow + TopRow);

            ChangeCount++;
        }

        public void ProtectCharacter(int protect)
        {
            LogController("ProtectChar(protect:" + protect + ")");
            CursorState.Attributes.Protected = protect;
        }

        public void RepeatLastCharacter(int count)
        {
            LogController("RepeatLastCharacter(count:" + count.ToString() + ")");

            if (LastCharacter == null)
            {
                LogController("  RepeatLastCharacter() doesn't make sense, not character in hold");
                return;
            }

            var character = LastCharacter.Clone();
            for(var i=0; i<count; i++)
            {
                PutChar(character.Char);
                if(!string.IsNullOrEmpty(character.CombiningCharacters))
                {
                    foreach (var ch in character.CombiningCharacters)
                        PutChar(ch);
                }
            }

            // The following line is to make this pass vttest for REP. It claims this
            // is not necessary as ECMA doesn't define otherwise, but XTerm handles it
            // like this.
            LastCharacter = null;
        }

        public void PutChar(char character)
        {
            LogExtreme("PutChar(ch:'" + character + "'=" + ((int)character).ToString() + ")");

            if (!CursorState.Utf8 && IsRGrCharacter(character))
                character = Iso2022Encoding.DecodeChar((char)(character - (char)0x80), RightCharacterSet, CursorState.NationalCharacterReplacementMode);
            else
                character = Iso2022Encoding.DecodeChar(character, CharacterSet, CursorState.NationalCharacterReplacementMode);

            if (CursorState.SingleShiftSelectCharacterMode != ECharacterSetMode.Unset)
            {
                CursorState.CharacterSetMode = CursorState.SingleShiftSelectCharacterMode;
                CursorState.SingleShiftSelectCharacterMode = ECharacterSetMode.Unset;
            }

            if (StoreRawText)
            {
                if (_rawText == null)
                    _rawText = new char[1024];
                else if ((_rawTextLength + 1) >= _rawText.Length)
                    Array.Resize(ref _rawText, _rawText.Length * 2);

                _rawText[_rawTextLength++] = character;
            }

            if (IsCombiningCharacter(character) && CursorState.CurrentColumn > 0)
            {
                // TODO : Find a better solution to ensure that combining marks work
                var changedCharacter = SetCombiningCharacter(CursorState.CurrentColumn - 1, CursorState.CurrentRow, character);
                if(changedCharacter != null)
                    LastCharacter = changedCharacter.Clone();

                return;
            }

            if (CursorState.InsertMode == EInsertReplaceMode.Insert)
            {
                while (Buffer.Count <= (TopRow + CursorState.CurrentRow))
                    Buffer.Add(new TerminalLine());

                var line = Buffer[TopRow + CursorState.CurrentRow];
                while (line.Count < CursorState.CurrentColumn)
                    line.Add(new TerminalCharacter());

                line.Insert(CursorState.CurrentColumn, new TerminalCharacter());
            }

            if (CursorState.CurrentColumn >= CurrentLineColumns && CursorState.WordWrap)
            {
                CursorState.CurrentColumn = 0;
                NewLine();
            }

            
            LastCharacter = SetCharacter(CursorState.CurrentColumn, CursorState.CurrentRow, character, CursorState.Attributes).Clone();
            CursorState.CurrentColumn++;

            if (CursorState.CurrentColumn >= CurrentLineColumns && !CursorState.WordWrap)
                CursorState.CurrentColumn = CurrentLineColumns - 1;

            var lineToClip = Buffer[TopRow + CursorState.CurrentRow];
            while (lineToClip.Count > CurrentLineColumns)
                lineToClip.RemoveAt(lineToClip.Count - 1);

            ChangeCount++;
        }

        /// <summary>
        /// Returns whether the character is considered to be "Right Graphics"
        /// </summary>
        /// <todo>
        /// I believe this is good enough for now.
        /// </todo>
        /// <param name="character">The character to test</param>
        /// <returns>True if in the high character region</returns>
        private bool IsRGrCharacter(char character)
        {
            return
                character >= (char)0xA0 &&
                character <= (char)0xFF;
        }

        public void PutG2Char(char character)
        {
            LogExtreme("PutG2Char(ch:'" + character + "'=" + ((int)character).ToString() + ")");

            var oldUtf8 = CursorState.Utf8;
            var oldMode = CursorState.CharacterSetMode;
            CursorState.CharacterSetMode = ECharacterSetMode.IsoG2;
            CursorState.Utf8 = false;

            PutChar(character);
            CursorState.CharacterSetMode = oldMode;
            CursorState.Utf8 = oldUtf8;
        }

        public void PutG3Char(char character)
        {
            LogExtreme("PutG3Char(ch:'" + character + "'=" + ((int)character).ToString() + ")");

            var oldMode = CursorState.CharacterSetMode;
            CursorState.CharacterSetMode = ECharacterSetMode.IsoG3;

            PutChar(character);
            CursorState.CharacterSetMode = oldMode;
        }

        public void SetWindowTitle(string title)
        {
            LogController("SetWindowTitle(t:'" + title + "')");

            WindowTitle = title;
            WindowTitleChanged?.Invoke(this, new TextEventArgs { Text = title });
        }

        public void ShiftIn()
        {
            LogController("ShiftIn()");
            CursorState.Utf8 = false;
            CursorState.CharacterSetMode = ECharacterSetMode.IsoG0;
        }

        public void ShiftOut()
        {
            LogController("ShiftOut()");
            CursorState.Utf8 = false;
            CursorState.CharacterSetMode = ECharacterSetMode.IsoG1;
        }

        public void SingleShiftSelectG2()
        {
            LogController("SingleShiftSelectG2()");
            CursorState.SingleShiftSelectCharacterMode = CursorState.CharacterSetMode;
            CursorState.CharacterSetMode = ECharacterSetMode.IsoG2;
        }

        public void SingleShiftSelectG3()
        {
            LogController("SingleShiftSelectG3()");
            CursorState.SingleShiftSelectCharacterMode = CursorState.CharacterSetMode;
            CursorState.CharacterSetMode = ECharacterSetMode.IsoG3;
        }

        public void InvokeCharacterSetMode(ECharacterSetMode mode)
        {
            LogController("InvokeCharacterSetMode(mode: " + mode.ToString() + ")");

            CursorState.CharacterSetMode = mode;
        }

        public void InvokeCharacterSetModeR(ECharacterSetMode mode)
        {
            LogController("InvokeCharacterSetModeR(mode: " + mode.ToString() + ")");

            CursorState.Utf8 = false;
            CursorState.CharacterSetModeR = mode;
        }

        public void SetRgbForegroundColor(int red, int green, int blue)
        {
            LogController("SetRgbForegroundColor(r:" + red + ", g:" + green + ", b:" + blue + ")");

            if (CursorState.Attributes.ForegroundRgb == null)
                CursorState.Attributes.ForegroundRgb = new TerminalColor { Red = (uint)red, Green = (uint)green, Blue = (uint)blue };
            else
                CursorState.Attributes.ForegroundRgb.Set((uint)red, (uint)green, (uint)blue);
        }

        public void SetRgbBackgroundColor(int red, int green, int blue)
        {
            LogController("SetRgbBackgroundColor(r:" + red + ", g:" + green + ", b:" + blue + ")");

            if (CursorState.Attributes.BackgroundRgb == null)
                CursorState.Attributes.BackgroundRgb = new TerminalColor { Red = (uint)red, Green = (uint)green, Blue = (uint)blue };
            else
                CursorState.Attributes.BackgroundRgb.Set((uint)red, (uint)green, (uint)blue);
        }

        public void SetIso8613PaletteForeground(int paletteEntry)
        {
            LogController("SetIso8613PaletteForeground(e:" + paletteEntry + ")");
            if(TerminalColor.Iso8613.TryGetValue(paletteEntry, out TerminalColor color))
            {
                if (CursorState.Attributes.ForegroundRgb == null)
                    CursorState.Attributes.ForegroundRgb = new TerminalColor(color);
                else
                    CursorState.Attributes.ForegroundRgb.ARGB = color.ARGB;
            }
        }

        public void SetIso8613PaletteBackground(int paletteEntry)
        {
            LogController("SetIso8613PaletteBackground(e:" + paletteEntry + ")");
            if (TerminalColor.Iso8613.TryGetValue(paletteEntry, out TerminalColor color))
            {
                if (CursorState.Attributes.BackgroundRgb == null)
                    CursorState.Attributes.BackgroundRgb = new TerminalColor(color);
                else
                    CursorState.Attributes.BackgroundRgb.ARGB = color.ARGB;
            }
        }

        public void SetCharacterAttribute(int parameter)
        {
            switch (parameter)
            {
                case 0:
                    LogController("SetCharacterAttribute(reset)");
                    CursorState.Attributes.ForegroundRgb = null;
                    CursorState.Attributes.BackgroundRgb = null;
                    CursorState.Attributes.ForegroundColor = ETerminalColor.White;
                    CursorState.Attributes.BackgroundColor = ETerminalColor.Black;
                    CursorState.Attributes.Bright = false;
                    CursorState.Attributes.Standout = false;
                    CursorState.Attributes.Underscore = false;
                    CursorState.Attributes.Blink = false;
                    CursorState.Attributes.Reverse = false;
                    CursorState.Attributes.Hidden = false;
                    break;

                case 1:
                    LogController("SetCharacterAttribute(bright)");
                    CursorState.Attributes.Bright = true;
                    break;

                case 2:
                    LogController("SetCharacterAttribute(dim)");
                    CursorState.Attributes.Bright = false;
                    break;

                case 3:
                    LogController("SetCharacterAttribute(standout)");
                    CursorState.Attributes.Standout = true;
                    break;

                case 4:
                    LogController("SetCharacterAttribute(underscore)");
                    CursorState.Attributes.Underscore = true;
                    break;

                case 5:
                    LogController("SetCharacterAttribute(blink)");
                    CursorState.Attributes.Blink = true;
                    break;

                case 7:
                    LogController("SetCharacterAttribute(reverse)");
                    CursorState.Attributes.Reverse = true;
                    break;

                case 8:
                    LogController("SetCharacterAttribute(hidden)");
                    CursorState.Attributes.Hidden = true;
                    break;

                case 22:
                    LogController("SetCharacterAttribute(not bright)");
                    CursorState.Attributes.Bright = false;
                    break;

                case 24:
                    LogController("SetCharacterAttribute(not underlined)");
                    CursorState.Attributes.Underscore = false;
                    break;

                case 25:
                    LogController("SetCharacterAttribute(steady)");
                    CursorState.Attributes.Blink = false;
                    break;

                case 27:
                    LogController("SetCharacterAttribute(not reverse)");
                    CursorState.Attributes.Reverse = false;
                    break;

                case 28:
                    LogController("SetCharacterAttribute(not hidden)");
                    CursorState.Attributes.Hidden = false;
                    break;

                case 30:
                case 31:
                case 32:
                case 33:
                case 34:
                case 35:
                case 36:
                case 37:
                case 38:
                    CursorState.Attributes.ForegroundRgb = null;
                    CursorState.Attributes.ForegroundColor = (ETerminalColor)(parameter - 30);
                    LogController("SetCharacterAttribute(foreground:" + CursorState.Attributes.ForegroundColor.ToString() + ")");
                    break;
                case 39:
                    CursorState.Attributes.ForegroundRgb = null;
                    CursorState.Attributes.ForegroundColor = ETerminalColor.White;
                    LogController("SetCharacterAttribute(foreground:default)");
                    break;
                case 40:
                case 41:
                case 42:
                case 43:
                case 44:
                case 45:
                case 46:
                case 47:
                case 48:
                    CursorState.Attributes.BackgroundRgb = null;
                    CursorState.Attributes.BackgroundColor = (ETerminalColor)(parameter - 40);
                    LogController("SetCharacterAttribute(background:" + CursorState.Attributes.BackgroundColor.ToString() + ")");
                    break;
                case 49:
                    CursorState.Attributes.BackgroundRgb = null;
                    CursorState.Attributes.BackgroundColor = ETerminalColor.Black;
                    LogController("SetCharacterAttribute(background:default)");
                    break;

                case 90:
                case 91:
                case 92:
                case 93:
                case 94:
                case 95:
                case 96:
                case 97:
                    CursorState.Attributes.ForegroundRgb = new TerminalColor((ETerminalColor)(parameter - 90), true);
                    LogController("SetCharacterAttribute(foregroundRgb:" + CursorState.Attributes.ForegroundRgb.ToString() + ")");
                    break;

                case 100:
                case 101:
                case 102:
                case 103:
                case 104:
                case 105:
                case 106:
                case 107:
                    CursorState.Attributes.BackgroundRgb = new TerminalColor((ETerminalColor)(parameter - 100), true);
                    LogController("SetCharacterAttribute(backgroundRgb:" + CursorState.Attributes.BackgroundRgb.ToString() + ")");
                    break;

                default:
                    LogController("SetCharacterAttribute(parameter:" + parameter + ")");
                    break;
            }
        }

        public void SetCharacterSize(ECharacterSize size)
        {
            LogController("SetCharacterSize(size:" + size.ToString() + ")");

            while ((CursorState.CurrentRow + TopRow) >= Buffer.Count)
                Buffer.Add(new TerminalLine());
            var currentLine = Buffer[CursorState.CurrentRow + TopRow];

            switch (size)
            {
                default:
                case ECharacterSize.SingleWidthLine:
                    currentLine.DoubleWidth = false;
                    currentLine.DoubleHeightTop = false;
                    currentLine.DoubleHeightBottom = false;
                    break;
                case ECharacterSize.DoubleHeightLineTop:
                    currentLine.DoubleWidth = true;
                    currentLine.DoubleHeightBottom = false;
                    currentLine.DoubleHeightTop = true;
                    break;
                case ECharacterSize.DoubleHeightLineBottom:
                    currentLine.DoubleWidth = true;
                    currentLine.DoubleHeightTop = false;
                    currentLine.DoubleHeightBottom = true;
                    break;
                case ECharacterSize.DoubleWidthLine:
                    currentLine.DoubleHeightTop = false;
                    currentLine.DoubleHeightBottom = false;
                    currentLine.DoubleWidth = true;
                    break;
                case ECharacterSize.ScreenAlignmentTest:
                    ScreenAlignmentTest();
                    break;
            }
        }

        public void ScreenAlignmentTest()
        {
            var attribute = new TerminalAttribute();
            for (var y = 0; y < VisibleRows; y++)
                for (var x = 0; x < VisibleColumns; x++)
                    SetCharacter(x, y, 'E', attribute);
        }

        public void SaveCursor()
        {
            LogController("SaveCursor()");

            SavedCursorState = CursorState.Clone();

            LogController("     C=" + CursorState.CurrentColumn.ToString() + ",R=" + CursorState.CurrentRow.ToString());
        }

        public void RestoreCursor()
        {
            LogController("RestoreCursor()");

            if (SavedCursorState != null)
                CursorState = SavedCursorState.Clone();

            LogController("     C=" + CursorState.CurrentColumn.ToString() + ",R=" + CursorState.CurrentRow.ToString());
        }

        public void EnableNormalBuffer()
        {
            LogController("EnableNormalBuffer()");

            if (ActiveBuffer == EActiveBuffer.Normal)
                return;

            ActiveBuffer = EActiveBuffer.Normal;
            Buffer = normalBuffer;

            alternativeBufferTopRow = TopRow;
            TopRow = normalBufferTopRow;

            ChangeCount++;
        }

        public void EnableAlternateBuffer()
        {
            LogController("EnableAlternateBuffer()");

            if (ActiveBuffer == EActiveBuffer.Alternative)
                return;

            ActiveBuffer = EActiveBuffer.Alternative;
            Buffer = alternativeBuffer;

            normalBufferTopRow = TopRow;
            TopRow = alternativeBufferTopRow;

            ChangeCount++;
        }

        public void UseHighlightMouseTracking(bool enable)
        {
            LogController("Unimplemented: UseHighlightMouseTracking(enable:" + enable.ToString() + ")");
            HighlightMouseTracking = enable;
            ChangeCount++;
        }

        public void UseCellMotionMouseTracking(bool enable)
        {
            LogController("UseCellMotionMouseTracking(enable:" + enable.ToString() + ")");
            CellMotionMouseTracking = enable;
            if(enable)
            {
                UseAllMouseTracking = false;
                HighlightMouseTracking = false;
                SgrMouseMode = false;
                X10SendMouseXYOnButton = false;
                X11SendMouseXYOnButton = false;
            }

            LastMousePosition.Set(-1, -1);
            ChangeCount++;
        }

        public void EnableSgrMouseMode(bool enable)
        {
            LogController("EnableSgrMouseMode(enable:" + enable.ToString() + ")");
            SgrMouseMode = enable;
            if (enable)
            {
                Utf8MouseMode = false;
                UrxvtMouseMode = false;
                UseAllMouseTracking = false;
                CellMotionMouseTracking = false;
                HighlightMouseTracking = false;
                X10SendMouseXYOnButton = false;
                X11SendMouseXYOnButton = false;
            }

            LastMousePosition.Set(-1, -1);
            ChangeCount++;
        }

        public void EnableUrxvtMouseMode(bool enabled)
        {
            LogController("EnableUrxvtMouseMode(enabled:" + enabled.ToString() + ")");
            UrxvtMouseMode = enabled;

            if(enabled)
            {
                Utf8MouseMode = false;
                SgrMouseMode = false;
            }

            LastMousePosition.Set(-1, -1);
            ChangeCount++;
        }

        public void SaveEnableNormalBuffer()
        {
            LogController("Unimplemented: SaveEnableNormalBuffer()");
        }

        public void RestoreEnableNormalBuffer()
        {
            LogController("Unimplemented: RestoreEnableNormalBuffer()");
        }

        public void SaveUseHighlightMouseTracking()
        {
            LogController("Unimplemented: SaveUseHighlightMouseTracking()");
        }

        public void RestoreUseHighlightMouseTracking()
        {
            LogController("Unimplemented: RestoreUseHighlightMouseTracking()");
        }

        public void SaveUseCellMotionMouseTracking()
        {
            LogController("Unimplemented: SaveUseCellMotionMouseTracking()");
        }

        public void RestoreUseCellMotionMouseTracking()
        {
            LogController("Unimplemented: RestoreUseCellMotionMouseTracking()");
        }

        public void SaveEnableSgrMouseMode()
        {
            LogController("Unimplemented: SaveEnableSgrMouseMode()");
        }

        public void RestoreEnableSgrMouseMode()
        {
            LogController("Unimplemented: RestoreEnableSgrMouseMode()");
        }

        public void SetBracketedPasteMode(bool enable)
        {
            LogController("SetBracketedPasteMode(enable:" + enable.ToString() + ")");
            BracketedPasteMode = enable;
        }

        public void SaveBracketedPasteMode()
        {
            LogController("Unimplemented: SaveBracketedPasteMode()");
        }

        public void RestoreBracketedPasteMode()
        {
            LogController("Unimplemented: RestoreBracketedPasteMode()");
        }

        public void SetInsertReplaceMode(EInsertReplaceMode mode)
        {
            LogController("SetInsertReplaceMode(mode:" + mode.ToString() + ")");
            CursorState.InsertMode = mode;
        }

        public void ClearScrollingRegion()
        {
            LogController("ClearScrollingRegion()");
            ScrollTop = 0;
            ScrollBottom = -1;
        }

        public void SetAutomaticNewLine(bool enable)
        {
            LogController("SetAutomaticNewLine(enable:" + enable.ToString() + ")");
            CursorState.AutomaticNewLine = enable;
        }

        public void EnableApplicationCursorKeys(bool enable)
        {
            LogController("EnableApplicationCursorKeys(enable:" + enable.ToString() + ")");
            CursorState.ApplicationCursorKeysMode = enable;
        }

        public void SaveCursorKeys()
        {
            LogController("Unimplemented: SaveCursorKeys()");
        }

        public void RestoreCursorKeys()
        {
            LogController("Unimplemented: RestoreCursorKeys()");
        }

        public void SetKeypadType(EKeypadType type)
        {
            LogController("Unimplemented: SetKeypadType(type:" + type.ToString() + ")");
        }

        public void SetScrollingRegion(int top, int bottom)
        {
            LogController("SetScrollingRegion(top:" + top.ToString() + ",bottom:" + bottom.ToString() + ")");

            if (bottom < top)
                return;

            if (top == 1 && bottom == VisibleRows)
                ClearScrollingRegion();
            else
            {
                ScrollTop = top - 1;
                ScrollBottom = bottom - 1;

                if (CursorState.OriginMode)
                    CursorState.CurrentRow = ScrollTop;
            }
        }

        public void SetLeftAndRightMargins(int left, int right)
        {
            LogController("SetLeftAndRightMargins(left:" + left.ToString() + ",right:" + right.ToString() + ")");

            if (!LeftAndRightMarginEnabled)
                return;

            LeftMargin = left - 1;
            RightMargin = right - 1;

            if (CursorState.OriginMode)
                SetCursorPosition(1, 1);
        }

        public void EraseLine(bool ignoreProtected = true)
        {
            LogController("EraseLine(ignoreProtected: " + ignoreProtected + ")");

            for (var i = 0; i < Columns; i++)
                SetCharacter(i, CursorState.CurrentRow, ' ', CursorState.Attributes, ignoreProtected);

            var line = Buffer[TopRow + CursorState.CurrentRow];
            while (line.Count > Columns)
                line.RemoveAt(line.Count - 1);

            ChangeCount++;
        }

        public void EraseToEndOfLine(bool ignoreProtected=true)
        {
            LogController("EraseToEndOfLine(ignoreProtected: " + ignoreProtected + ")");

            for (var i = CursorState.CurrentColumn; i < Columns; i++)
                SetCharacter(i, CursorState.CurrentRow, ' ', CursorState.Attributes, ignoreProtected);

            var line = Buffer[TopRow + CursorState.CurrentRow];
            while (line.Count > Columns)
                line.RemoveAt(line.Count - 1);

            // Filling to the end of the line to adopt attributes for applications like Midnight commander
            while (line.Count < Columns)
            {
                line.Add(
                    new TerminalCharacter
                    {
                        Char = ' ',
                        Attributes = CursorState.Attributes
                    }
                );
            }

            ChangeCount++;
        }

        public void EraseToStartOfLine(bool ignoreProtected = true)
        {
            LogController("EraseToStartOfLine(ignoreProtected: " + ignoreProtected + ")");

            for (var i = 0; i < Columns && i <= CursorState.CurrentColumn; i++)
                SetCharacter(i, CursorState.CurrentRow, ' ', CursorState.Attributes, ignoreProtected);

            var line = Buffer[TopRow + CursorState.CurrentRow];
            while (line.Count > Columns)
                line.RemoveAt(line.Count - 1);

            ChangeCount++;
        }

        public void EraseBelow(bool ignoreProtected = true)
        {
            // TODO : Optimize
            LogController("EraseBelow(ignoreProtected: " + ignoreProtected + ")");

            for (var y = CursorState.CurrentRow + 1; y < VisibleRows; y++)
            {
                for (var x = 0; x < VisibleColumns; x++)
                    SetCharacter(x, y, ' ', CursorState.Attributes, ignoreProtected);

                var line = Buffer[TopRow + y];
                while (line.Count > Columns)
                    line.RemoveAt(line.Count - 1);
            }


            for (var x = CursorState.CurrentColumn; x < VisibleColumns; x++)
                SetCharacter(x, CursorState.CurrentRow, ' ', CursorState.Attributes, ignoreProtected);
        }

        public void EraseAbove(bool ignoreProtected = true)
        {
            // TODO : Optimize
            LogController("EraseAbove(ignoreProtected: " + ignoreProtected + ")");

            for (var y = CursorState.CurrentRow - 1; y >= 0; y--)
            {
                for (var x = 0; x < VisibleColumns; x++)
                    SetCharacter(x, y, ' ', CursorState.Attributes, ignoreProtected);

                var line = Buffer[TopRow + y];
                while (line.Count > Columns)
                    line.RemoveAt(line.Count - 1);
            }

            for (var x = 0; x <= CursorState.CurrentColumn; x++)
                SetCharacter(x, CursorState.CurrentRow, ' ', CursorState.Attributes, ignoreProtected);
        }

        public void DeleteLines(int count)
        {
            // TODO : Verify it works with scroll range
            LogController("DeleteLines(count:" + count.ToString() + ")");

            if (
                CursorState.CurrentRow < ScrollTop ||
                (ScrollBottom >= 0 && CursorState.CurrentRow > ScrollBottom)
            )
                return;

            if (
                LeftAndRightMarginEnabled &&
                (
                    CursorState.CurrentColumn < LeftMargin ||
                    CursorState.CurrentColumn > RightMargin
                )
            )
                return;

            if ((CursorState.CurrentRow + TopRow) >= Buffer.Count)
                return;

            if (LeftAndRightMarginEnabled)
            {
                var scrollTop = CursorState.CurrentRow;
                var scrollBottom = ScrollBottom;
                if (scrollBottom == +1)
                    scrollBottom = Rows - 1;

                if (scrollTop < scrollBottom)
                    ScrollVisualRect(LeftMargin, scrollTop, RightMargin, scrollBottom, count);
            }
            else
            {
                int lineToInsert = TopRow + VisibleRows;
                if (ScrollBottom != -1)
                    lineToInsert = TopRow + ScrollBottom;

                while ((count--) > 0)
                {
                    if ((CursorState.CurrentRow + TopRow) < Buffer.Count)
                    {
                        Buffer.RemoveAt(CursorState.CurrentRow + TopRow);

                        if (lineToInsert <= Buffer.Count)
                            Buffer.Insert(lineToInsert, new TerminalLine());
                    }
                }
            }

            ChangeCount++;
        }

        private void InsertBlanks(int count, int row)
        {
            LogController("InsertBlank(count:" + count.ToString() + ")");

            if (
                LeftAndRightMarginEnabled &&
                (
                    CursorState.CurrentColumn < LeftMargin ||
                    CursorState.CurrentColumn > RightMargin
                )
            )
                return;

            while (Buffer.Count <= row)
                Buffer.Add(new TerminalLine());

            var line = Buffer[row];
            while (line.Count < CursorState.CurrentColumn)
                line.Add(new TerminalCharacter { Attributes = NullAttribute.Clone() } );

            var removeAt = Columns;
            if (LeftAndRightMarginEnabled)
                removeAt = RightMargin + 1;

            for (var i = 0; i < count; i++)
            {
                line.Insert(CursorState.CurrentColumn, new TerminalCharacter { Attributes = NullAttribute.Clone() } );

                if (removeAt < line.Count)
                    line.RemoveAt(removeAt);
            }
        }

        public void DeleteCharacter(int count, int row)
        {
            LogController("DeleteCharacter(count:" + count.ToString() + ", row:" + row.ToString() + ")");

            if (
                LeftAndRightMarginEnabled &&
                (
                    CursorState.CurrentColumn < LeftMargin ||
                    CursorState.CurrentColumn > RightMargin
                )
            )
                return;

            if (row >= Buffer.Count)
                return;

            var line = Buffer[row];

            var insertAt = Columns + 1;
            if (LeftAndRightMarginEnabled)
                insertAt = RightMargin;

            while (count > 0 && CursorState.CurrentColumn < line.Count)
            {
                line.RemoveAt(CursorState.CurrentColumn);
                count--;

                if (insertAt <= line.Count)
                {
                    line.Insert(
                        insertAt,
                        new TerminalCharacter
                        {
                            Char = ' ',
                            Attributes = line[insertAt - 1].Attributes
                        }
                    );
                }
            }

            ChangeCount++;
        }

        public void InsertColumn(int count)
        {
            LogController("InsertColumn(count:" + count.ToString() + ")");

            if (
                CursorState.CurrentRow < ScrollTop ||
                (ScrollBottom >= 0 && CursorState.CurrentRow > ScrollBottom)
            )
                return;

            if (
                LeftAndRightMarginEnabled &&
                (
                    CursorState.CurrentColumn < LeftMargin ||
                    CursorState.CurrentColumn > RightMargin
                )
            )
                return;

            var insertTop = CursorState.CurrentRow;
            var insertBottom = ScrollBottom;
            if (insertBottom == -1)
                insertBottom = Rows - 1;

            if (insertTop < insertBottom)
            {
                for (var row = insertTop; row <= insertBottom; row++)
                    InsertBlanks(count, row + TopRow);
            }

            ChangeCount++;
        }

        public void DeleteColumn(int count)
        {
            LogController("InsertColumn(count:" + count.ToString() + ")");

            if (
                CursorState.CurrentRow < ScrollTop ||
                (ScrollBottom >= 0 && CursorState.CurrentRow > ScrollBottom)
            )
                return;

            if (
                LeftAndRightMarginEnabled &&
                (
                    CursorState.CurrentColumn < LeftMargin ||
                    CursorState.CurrentColumn > RightMargin
                )
            )
                return;

            var insertTop = CursorState.CurrentRow;
            var insertBottom = ScrollBottom;
            if (insertBottom == -1)
                insertBottom = Rows - 1;

            if (insertTop < insertBottom)
            {
                for (var row = insertTop; row <= insertBottom; row++)
                    DeleteCharacter(count, row + TopRow);
            }

            ChangeCount++;
        }

        public void InsertLines(int count)
        {
            LogController("InsertLines(count:" + count.ToString() + ")");

            if (
                CursorState.CurrentRow < ScrollTop ||
                (ScrollBottom >= 0 && CursorState.CurrentRow > ScrollBottom)
            )
                return;

            if (
                LeftAndRightMarginEnabled &&
                (
                    CursorState.CurrentColumn < LeftMargin ||
                    CursorState.CurrentColumn > RightMargin
                )
            )
                return;

            if ((CursorState.CurrentRow + TopRow) >= Buffer.Count)
                return;

            if (LeftAndRightMarginEnabled)
            {
                var scrollTop = CursorState.CurrentRow;
                var scrollBottom = ScrollBottom;
                if (scrollBottom == +1)
                    scrollBottom = Rows - 1;

                if (scrollTop < scrollBottom)
                    ScrollVisualRect(LeftMargin, scrollTop, RightMargin, scrollBottom, -count);
            }
            else
            {
                int lineToRemove = TopRow + VisibleRows;
                if (ScrollBottom != -1)
                    lineToRemove = TopRow + ScrollBottom;

                while ((count--) > 0)
                {
                    if (lineToRemove < Buffer.Count)
                        Buffer.RemoveAt(lineToRemove);

                    Buffer.Insert((CursorState.CurrentRow + TopRow), new TerminalLine());
                }
            }

            ChangeCount++;
        }

        public void EraseAll(bool ignoreProtected = true)
        {
            LogController("EraseAll(ignoreProtected: " + ignoreProtected + ")");

            NullAttribute = CursorState.Attributes.Clone();

            if(!ignoreProtected || GuardedArea != null)
            {
                EraseAbove(ignoreProtected);
                EraseBelow(ignoreProtected);
                return;
            }

            TopRow = Buffer.Count;
            while(TopRow > MaximumHistoryLines)
            {
                Buffer.RemoveAt(0);
                TopRow--;
            }

            Columns = CursorState.ConfiguredColumns == 0 ? VisibleColumns : CursorState.ConfiguredColumns;
            Rows = VisibleRows;

            CursorState.Utf8 = true;

            // TODO : This is hackish as it makes the 80 and 132 column mode stick only until the next erase
            CursorState.ConfiguredColumns = 0;      

            ChangeCount++;
        }

        public void Enable132ColumnMode(bool enable)
        {
            LogController("Enable132ColumnMode(enable:" + enable.ToString() + ")");
            EraseAll();
            Columns = enable ? 132 : 80;
            CursorState.ConfiguredColumns = Columns;
            SetCursorPosition(1, 1);
        }

        public void EnableSmoothScrollMode(bool enable)
        {
            LogController("Unimplemented: EnableSmoothScrollMode(enable:" + enable.ToString() + ")");
            SmoothScrollMode = enable;
        }

        public void EnableReverseVideoMode(bool enable)
        {
            LogController("EnableReverseVideoMode(enable:" + enable.ToString() + ")");
            CursorState.ReverseVideoMode = enable;

            ChangeCount++;
        }

        public void EnableBlinkingCursor(bool enable)
        {
            LogController("EnableBlinkingCursor(enable:" + enable.ToString() + ")");
            CursorState.BlinkingCursor = enable;

            ChangeCount++;
        }

        public void ShowCursor(bool show)
        {
            LogController("ShowCursor(show:" + show.ToString() + ")");
            CursorState.ShowCursor = show;

            ChangeCount++;
        }

        public void EnableOriginMode(bool enable)
        {
            LogController("EnableOriginMode(enable:" + enable.ToString() + ")");
            CursorState.OriginMode = enable;
            SetCursorPosition(1, 1);
        }

        public void EnableWrapAroundMode(bool enable)
        {
            LogController("EnableWrapAroundMode(enable:" + enable.ToString() + ")");
            CursorState.WordWrap = enable;
        }

        public void EnableAutoRepeatKeys(bool enable)
        {
            LogController("Unimplemented: EnableAutoRepeatKeys(enable:" + enable.ToString() + ")");
        }

        public void Enable80132Mode(bool enable)
        {
            LogController("Unimplemented: Enable80132Mode(enable:" + enable.ToString() + ")");
            Columns = VisibleColumns;
        }

        public void EnableReverseWrapAroundMode(bool enable)
        {
            LogController("Unimplemented: EnableReverseWrapAroundMode(enable:" + enable.ToString() + ")");
            ReverseWrapAroundMode = enable;
        }

        public void EnableLeftAndRightMarginMode(bool enable)
        {
            LogController("EnableLeftAndRightMarginMode(enable:" + enable.ToString() + ")");

            if (LeftAndRightMarginEnabled != enable)
            {
                if (enable)
                {
                    LeftMargin = 0;
                    RightMargin = Columns;
                }
                LeftAndRightMarginEnabled = enable;
            }
        }

        public static readonly string DeviceAttributes = "\u001b[?64;1;2;6;9;15;18;21;22c";

        public void SendDeviceAttributes()
        {
            LogController("SendDeviceAttributes()");
            SendData?.Invoke(this, new SendDataEventArgs { Data = Encoding.ASCII.GetBytes(DeviceAttributes) });
        }

        public static readonly string XTermSecondaryAttributes = "\u001b[>41;136;0c";

        public void SendDeviceAttributesSecondary()
        {
            LogController("SendDeviceAttributesSecondary()");
            SendData?.Invoke(this, new SendDataEventArgs { Data = Encoding.ASCII.GetBytes(XTermSecondaryAttributes) });
        }

        public static readonly byte[] DsrOk = { 0x1B, (byte)'[', (byte)'0', (byte)'n' };

        public void DeviceStatusReport()
        {
            LogController("DeviceStatusReport()");
            SendData?.Invoke(this, new SendDataEventArgs { Data = DsrOk });
        }

        public void ReportCursorPosition()
        {
            LogController("ReportCursorPosition()");

            var rcp = "\u001b[" + (CursorState.CurrentRow - ScrollTop + 1).ToString() + ";" + (CursorState.CurrentColumn - LeftMargin + 1).ToString() + "R";

            SendData?.Invoke(this, new SendDataEventArgs { Data = Encoding.UTF8.GetBytes(rcp) });
        }

        public void ReportExtendedCursorPosition()
        {
            LogController("ReportExtendedCursorPosition()");

            var rcp = "\u001b[?" + (CursorState.CurrentRow - ScrollTop + 1).ToString() + ";" + (CursorState.CurrentColumn - LeftMargin + 1).ToString() + "R";

            SendData?.Invoke(this, new SendDataEventArgs { Data = Encoding.UTF8.GetBytes(rcp) });
        }

        public void SetLatin1()
        {
            LogController("Unimplemented: SetLatin1()");
            CursorState.Utf8 = false;
        }

        public void SetUTF8()
        {
            LogController("Unimplemented: SetUTF8()");
            CursorState.Utf8 = true;
        }

        public void ResizeView(int columns, int rows)
        {
            VisibleColumns = columns;
            VisibleRows = rows;
            Columns = columns;
            Rows = rows;

            if (CursorState.CurrentRow >= Rows)
            {
                var offset = CursorState.CurrentRow - Rows + 1;
                TopRow += offset;
                CursorState.CurrentRow -= offset;

                while (TopRow > MaximumHistoryLines)
                {
                    Buffer.RemoveAt(0);
                    TopRow--;
                }
            }

            var start = ((CursorState.TabStops.Count > 0) ? CursorState.TabStops.Last() : 0) & ~7;
            for (var t = start + 8; t <= Columns; t += 8)
                CursorState.TabStops.Add(t);

            while (CursorState.TabStops.Count > 0 && CursorState.TabStops.Last() > (Columns + 1))
                CursorState.TabStops.RemoveAt(CursorState.TabStops.Count - 1);

            if (SizeChanged != null)
                SizeChanged.Invoke(this, new SizeEventArgs { Width = columns, Height = Rows });
        }

        internal void TestPatternScrolling()
        {
            TopRow = 100;
            for (var y = 0; y < Rows; y++)
                for (var x = 0; x < Columns; x++)
                    SetCharacter(x, y, (char)('A' + y), CursorState.Attributes);
        }

        internal void TestPatternScrollingLower()
        {
            TopRow = 100;
            for (var y = 0; y < Rows; y++)
                for (var x = 0; x < Columns; x++)
                    SetCharacter(x, y, (char)('a' + y), CursorState.Attributes);
        }

        internal void TestPatternScrollingDiagonalLower()
        {
            TopRow = 100;
            for (var y = 0; y < Rows; y++)
                for (var x = 0; x < Columns; x++)
                    SetCharacter(x, y, (char)('a' + Math.Abs(x - y) % 26), CursorState.Attributes);
        }

        internal void TestPatternScrollingDiagonalUpper()
        {
            TopRow = 100;
            for (var y = 0; y < Rows; y++)
                for (var x = 0; x < Columns; x++)
                    SetCharacter(x, y, (char)('A' + Math.Abs(x - y) % 26), CursorState.Attributes);
        }

        private void Send(byte[] value)
        {
            SendData?.Invoke(this, new SendDataEventArgs { Data = value });
        }

        private static readonly byte[] BracketedPasteModePrefix = Encoding.ASCII.GetBytes("\u001b[200~,");
        private static readonly byte[] BracketedPasteModePostfix = Encoding.ASCII.GetBytes("\u001b[200~,");

        public void Paste(byte [] toPaste)
        {
            if (BracketedPasteMode)
                Send(BracketedPasteModePrefix.Concat(toPaste).Concat(BracketedPasteModePostfix).ToArray());
            else
                Send(toPaste);
        }

        private TerminalCharacter SetCharacter(int currentColumn, int currentRow, char ch, TerminalAttribute attribute, bool overwriteProtected=true)
        {
            while (Buffer.Count < (currentRow + TopRow + 1))
                Buffer.Add(new TerminalLine());

            var line = Buffer[currentRow + TopRow];
            while (line.Count < (currentColumn + 1))
                line.Add(new TerminalCharacter { Char = ' ', Attributes = NullAttribute.Clone() });

            var character = line[currentColumn];
            if ((GuardedArea == null && overwriteProtected) || (!overwriteProtected && character.Attributes.Protected != 1) || (GuardedArea != null && !GuardedArea.Contains(currentColumn, currentRow)))
            {
                character.Char = ch;
                character.Attributes = CursorState.Attributes.Clone();
                character.CombiningCharacters = "";
            }

            return character;
        }

        private TerminalCharacter SetCombiningCharacter(int column, int row, char combiningCharacter)
        {
            var line = GetVisualLine(row);

            if (line != null && column < line.Count)
            {
                line[column].CombiningCharacters += combiningCharacter;
                return line[column];
            }

            return null;
        }

        private static byte [] DecPrivateModeResponse(int mode, bool response, bool always=false)
        {
            return Encoding.ASCII.GetBytes(
                    "\u001b[?" + mode.ToString() + ";" + ((always && !response) ? "4" : (response ? "1" : "2")) + "$y"
                );
        }

        private static byte[] DecUnknownPrivateModeResponse(int mode)
        {
            return Encoding.ASCII.GetBytes(
                    "\u001b[?" + mode.ToString() + ";0$y"
                );
        }

        public void SetConformanceLevel(int level, bool eightBit)
        {
            LogController("SetConformanceLevel(level:" + level + ", 8bit:" + eightBit + ")");

            if (level == 0)
                SetVt52Mode(true);
            else
                SetVt52Mode(false);
        }

        public void SetVt52Mode(bool enabled)
        {
            LogController("SetVt52Mode(enabled:" + enabled + ")");
            Vt52Mode = enabled;
            Vt52AnsiMode = false;

            if(!enabled)
            {
                CursorState.G0 = ECharacterSet.USASCII;
                CursorState.G1 = ECharacterSet.USASCII;
                CursorState.G2 = ECharacterSet.USASCII;
                CursorState.G3 = ECharacterSet.USASCII;
                CursorState.Vt300G1 = ECharacterSet.USASCII;
                CursorState.Vt300G2 = ECharacterSet.USASCII;
                CursorState.Vt300G3 = ECharacterSet.USASCII;
            }
        }

        public void Vt52EnterAnsiMode()
        {
            Vt52AnsiMode = true;
        }

        public void SetVt52AlternateKeypadMode(bool enabled)
        {
            LogController("SetVt52AlternateKeypadMode(enabled:" + enabled + ")");
            CursorState.Vt52AlternateKeypad = enabled;
        }


        public void SetVt52GraphicsMode(bool enabled)
        {
            LogController("SetVt52GraphicsMode(enabled:" + enabled + ")");
            CursorState.Vt52GraphicsMode = enabled;
        }

        public void SetX10SendMouseXYOnButton(bool enabled)
        {
            LogController("SetVt52GraphicsMode(enabled:" + enabled + ")");
            X10SendMouseXYOnButton = enabled;
            if (enabled)
            {
                UseAllMouseTracking = false;
                CellMotionMouseTracking = false;
                HighlightMouseTracking = false;
                SgrMouseMode = false;
                X11SendMouseXYOnButton = false;
            }

            LastMousePosition.Set(-1, -1);
        }

        public void SetSendFocusInAndFocusOutEvents(bool enabled)
        {
            LogController("SetSendFocusInAndFocusOutEvents(enabled:" + enabled + ")");
            SendFocusInAndFocusOutEvents = enabled;
        }

        public void SetUseAllMouseTracking(bool enabled)
        {
            LogController("SetUseAllMouseTracking(enabled:" + enabled + ")");
            UseAllMouseTracking = enabled;
            if (enabled)
            {
                CellMotionMouseTracking = false;
                HighlightMouseTracking = false;
                SgrMouseMode = false;
                X10SendMouseXYOnButton = false;
                X11SendMouseXYOnButton = false;
            }
            LastMousePosition.Set(-1, -1);
        }

        public void SetUtf8MouseMode(bool enabled)
        {
            LogController("SetUtf8MouseMode(enabled:" + enabled + ")");
            Utf8MouseMode = enabled;
            if (Utf8MouseMode)
            {
                UrxvtMouseMode = false;
                SgrMouseMode = false;
            }
        }

        public void SetX11SendMouseXYOnButton(bool enabled)
        {
            LogController("SetVt52GraphicsMode(enabled:" + enabled + ")");
            X11SendMouseXYOnButton = enabled;
            if (enabled)
            {
                UseAllMouseTracking = false;
                CellMotionMouseTracking = false;
                HighlightMouseTracking = false;
                SgrMouseMode = false;
                X10SendMouseXYOnButton = false;
            }

            LastMousePosition.Set(-1, -1);
        }

        public void SetStartOfGuardedArea()
        {
            LogController("SetStartOfGuardedArea()");

            if (GuardedArea == null)
            {
                GuardedArea = new TextRange
                {
                    Start = CursorState.Position.Clone(),
                    End = CursorState.Position.Clone()
                };
            }
            else
                GuardedArea.Start = CursorState.Position.Clone();
        }

        public void SetEndOfGuardedArea()
        {
            LogController("SetEndOfGuardedArea()");

            if (GuardedArea == null)
            {
                GuardedArea = new TextRange
                {
                    Start = CursorState.Position.Clone(),
                    End = CursorState.Position.Clone()
                };
            }
            else
                GuardedArea.End = CursorState.Position.Clone();
        }

        public void SetErasureMode(bool enabled)
        {
            LogController("SetErasureMode(enabled:" + enabled + ")");
            ErasureMode = enabled;
        }

        public void SetGuardedAreaTransferMode(bool enabled)
        {
            LogController("SetGuardedAreaTransferMode(enabled:" + enabled + ")");
            GuardedAreaTransferMode = enabled;
        }

        public void RequestDecPrivateMode(int mode)
        {
            LogController("RequestDecPrivateMode(mode:" + mode.ToString() + ")");

            if (SendData == null)
                return;

            switch (mode)
            {
                case 1:         // Ps = 1  -> Application Cursor Keys (DECCKM). | Ps = 1  -> Normal Cursor Keys (DECCKM).
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, CursorState.ApplicationCursorKeysMode) });
                    break;

                case 2:         // Ps = 2  -> Designate USASCII for character sets G0-G3 (DECANM), and set VT100 mode. | Designate VT52 mode (DECANM).
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, CursorState.ApplicationCursorKeysMode) });
                    break;

                case 3:         // Ps = 3  -> 132 Column Mode (DECCOLM). | Ps = 3  -> 80 Column Mode (DECCOLM).
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, CursorState.ConfiguredColumns == 132) });
                    break;

                case 4:         // Ps = 4  -> Smooth (Slow) Scroll (DECSCLM). | Ps = 4  -> Jump (Fast) Scroll (DECSCLM).
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, SmoothScrollMode) });
                    break;

                case 5:         // Ps = 5  -> Reverse Video (DECSCNM). | Ps = 5  -> Normal Video (DECSCNM).
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, CursorState.ReverseVideoMode) });
                    break;

                case 6:         // DECOM
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, CursorState.OriginMode) });
                    break;

                case 7:         // Ps = 7  -> Wraparound Mode (DECAWM). | Ps = 7  -> No Wraparound Mode (DECAWM).
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, CursorState.WordWrap) });
                    break;

                case 8:         // Ps = 8  -> No Auto-repeat Keys (DECARM).
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, false, true) });
                    break;

                case 9:         // Ps = 9  -> (Send|Don't send) Mouse X & Y on button press.
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, X10SendMouseXYOnButton) });
                    break;

                case 12:        // Blinking Cursor (AT&T 610).
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, CursorState.BlinkingCursor) });
                    break;

                case 25:        // DECSET
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, CursorState.ShowCursor) });
                    break;

                case 45:        // Ps = 4 5  -> Reverse-wraparound Mode. | Ps = 4 5  -> No Reverse-wraparound Mode.
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, ReverseWrapAroundMode) });
                    break;

                case 1000:      // Ps = 1 0 0 0  -> (Send|Don't send) Mouse X & Y on button press and release.
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, X11SendMouseXYOnButton) });
                    break;

                case 1001:      // Ps = 1 0 0 1  -> (Use|Don't use) Hilite Mouse Tracking.
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, HighlightMouseTracking) });
                    break;

                case 1002:      // Ps = 1 0 0 2  -> (Use|Don't use) Cell Motion Mouse Tracking.
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, CellMotionMouseTracking) });
                    break;

                case 1003:      // Ps = 1 0 0 3  -> (Use|Don't use) All Motion Mouse Tracking.
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, UseAllMouseTracking) });
                    break;

                case 1004:      // Ps = 1 0 0 4  -> (Send|Don't send) FocusIn/FocusOut events.
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, SendFocusInAndFocusOutEvents) });
                    break;

                case 1005:      // Ps = 1 0 0 5  -> (Enable|Disable) UTF-8 Mouse Mode.
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, Utf8MouseMode) });
                    break;

                case 1006:      // Ps = 1 0 0 6  -> (Enable|Disable) SGR Mouse Mode.
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, SgrMouseMode) });
                    break;

                case 1015:      // Ps = 1 0 1 5  -> (Enable|Disable) urxvt Mouse Mode.
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, UrxvtMouseMode) });
                    break;

                case 1049:      // Ps = 1 0 4 9  ->  Normal|Alternative screen buffer
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, ActiveBuffer == EActiveBuffer.Normal) });
                    break;

                case 2004:      // Ps = 2 0 0 4  -> Set bracketed paste mode.
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecPrivateModeResponse(mode, BracketedPasteMode) });
                    break;

                default:
                    SendData.Invoke(this, new SendDataEventArgs { Data = DecUnknownPrivateModeResponse(mode) });
                    break;
            }
        }

        public static readonly string ConformanceLevelResponse = "\u0090$r64\u009c";      // VT420 compliance?

        public void RequestStatusStringSetConformanceLevel()
        {
            LogController("RequestStatusStringSetConformanceLevel()");

            if (Vt52Mode)
                return;

            SendData?.Invoke(this, new SendDataEventArgs { Data = Encoding.ASCII.GetBytes(ConformanceLevelResponse) });
        }

        public void RequestStatusStringSetProtectionAttribute()
        {
            LogController("RequestStatusStringSetProtectionAttribute()");

            var result = "\u001bP1$r" + CursorState.Attributes.Protected + "\"q\u001b\\";

            // TODO : Is this for the current state or the character at the cursor position?

            SendData?.Invoke(this, new SendDataEventArgs { Data = Encoding.ASCII.GetBytes(result) });
        }

        public static readonly string Vt52Identification = "\u001b/Z";

        public void Vt52Identify()
        {
            LogController("Vt52Identify()");

            SendData?.Invoke(this, new SendDataEventArgs { Data = Encoding.ASCII.GetBytes(Vt52Identification) });
        }

        public void SetCursorStyle(ECursorShape shape, bool blink)
        {
            CursorState.CursorShape = shape;
            CursorState.BlinkingCursor = blink;
        }

        public bool KeyPressed(string key, bool controlPressed, bool shiftPressed)
        {
            if (SendData == null)
                return false;

            var code = GetKeySequence(key, controlPressed, shiftPressed);
            if (code != null)
            {
                SendData.Invoke(this, new SendDataEventArgs { Data = code });

                return true;
            }

            SendData.Invoke(this, new SendDataEventArgs { Data = Encoding.UTF8.GetBytes(key) });

            return true;
        }

        public byte [] GetKeySequence(string key, bool control, bool shift)
        {
            return KeyboardTranslations.GetKeySequence(key, control, shift, CursorState.ApplicationCursorKeysMode);
        }

        private static bool IsCombiningCharacter(char ch)
        {
            return 
                (ch >= '\u0300' && ch <= '\u036F') ||   // Combining diacritical marks
                (ch >= '\u1AB0' && ch <= '\u1ABE') ||   // Combining diacritical marks extended
                (ch >= '\u1DC0' && ch <= '\u1DFF') ||   // Combining diacritical marks supplement
                (ch >= '\u20D0' && ch <= '\u20F1') ||   // Combining diacritical marks for symbols
                (ch >= '\uFE20' && ch <= '\uFE2F')      // Combining half marks
                ;
        }

        /// <summary>
        /// Sends a mouse press event to the server when the appropriate mode is configured from the server
        /// </summary>
        /// <param name="x">X coordinate (0 based visual)</param>
        /// <param name="y">X coordinate (1 based visual)</param>
        /// <param name="buttonNumber">Button number (left=0, right=1, middle=2)</param>
        /// <param name="controlPressed">true if control is pressed</param>
        /// <param name="shiftPressed">true if shift is pressed</param>
        public void MousePress(int x, int y, int buttonNumber, bool controlPressed, bool shiftPressed)
        {
            if (SendData == null)
                return;

            if(X10SendMouseXYOnButton)
            {
                var x10Message = "\u001b[M" + (char)(buttonNumber + ' ') + (char)(' ' + x + 1) + (char)(' ' + y + 1);

                SendData.Invoke(this,
                    new SendDataEventArgs
                    {
                        Data = Utf8MouseMode ?
                            Encoding.UTF8.GetBytes(x10Message) :
                            x10Message.Select(s => (byte)(Math.Min(255, (int)s))).ToArray()
                    }
                );
            }

            if (X11SendMouseXYOnButton || CellMotionMouseTracking || UseAllMouseTracking)
            {
                var x11modifier =
                    (buttonNumber & 0x3) |
                    (controlPressed ? 16 : 0) |
                    (shiftPressed ? 4 : 0);

                var x11Message = "\u001b[M" + (char)(x11modifier + ' ') + (char)(' ' + x + 1) + (char)(' ' + y + 1);

                SendData.Invoke(this,
                    new SendDataEventArgs
                    {
                        Data = Utf8MouseMode ?
                            Encoding.UTF8.GetBytes(x11Message) :
                            x11Message.Select(s => (byte)(Math.Min(255, (int)s))).ToArray()
                    }
                );
            }

            if (SgrMouseMode)
            {
                var modifier =
                    (buttonNumber & 0x3) |
                    (controlPressed ? 16 : 0) |
                    (shiftPressed ? 4 : 0);

                var message = "\u001b[<" + modifier.ToString() + ";" + (x + 1).ToString() + ";" + (y + 1).ToString() + "M";

                SendData.Invoke(this,
                    new SendDataEventArgs
                    {
                        Data = Encoding.UTF8.GetBytes(message)
                    }
                );
            }
        }

        /// <summary>
        /// Sends a mouse press event to the server when the appropriate mode is configured from the server
        /// </summary>
        /// <param name="x">X coordinate (0 based visual)</param>
        /// <param name="y">X coordinate (1 based visual)</param>
        /// <param name="controlPressed">true if control is pressed</param>
        /// <param name="shiftPressed">true if shift is pressed</param>
        public void MouseRelease(int x, int y, bool controlPressed, bool shiftPressed)
        {
            if (SendData == null)
                return;

            LastMousePosition.Set(-1, -1);

            if (X11SendMouseXYOnButton || CellMotionMouseTracking ||UseAllMouseTracking)
            {
                var x11modifier =
                    (0x3) |
                    (controlPressed ? 16 : 0) |
                    (shiftPressed ? 4 : 0);

                var x11Message = "\u001b[M" + (char)(x11modifier + ' ') + (char)(' ' + x + 1) + (char)(' ' + y + 1);

                SendData.Invoke(this,
                    new SendDataEventArgs
                    {
                        Data = Utf8MouseMode ?
                            Encoding.UTF8.GetBytes(x11Message) :
                            x11Message.Select(s => (byte)(Math.Min(255, (int)s))).ToArray()
                    }
                );
            }

            if (SgrMouseMode)
            {
                var modifier =
                    (0x3) |
                    (controlPressed ? 16 : 0) |
                    (shiftPressed ? 4 : 0);

                var message = "\u001b[<" + modifier.ToString() + ";" + (x + 1).ToString() + ";" + (y + 1).ToString() + "m";

                SendData.Invoke(this,
                    new SendDataEventArgs
                    {
                        Data = Encoding.UTF8.GetBytes(message)
                    }
                );
            }
        }

        /// <summary>
        /// Sends a mouse move event to the server when the appropriate mode is configured from the server
        /// </summary>
        /// <param name="x">X coordinate (0 based visual)</param>
        /// <param name="y">X coordinate (1 based visual)</param>
        /// <param name="buttonNumber">Button number (left=0, right=1, middle=2, noButton=3)</param>
        /// <param name="controlPressed">true if control is pressed</param>
        /// <param name="shiftPressed">true if shift is pressed</param>
        public void MouseMove(int x, int y, int buttonNumber, bool controlPressed, bool shiftPressed)
        {
            if (SendData == null)
                return;

            if (LastMousePosition.Equals(x, y))
                return;

            if (CellMotionMouseTracking && (buttonNumber != 3) || UseAllMouseTracking)
            {
                var x11modifier =
                    (buttonNumber & 0x3) |
                    (controlPressed ? 16 : 0) |
                    (shiftPressed ? 4 : 0) |
                    32;

                var x11Message = "\u001b[M" + (char)(x11modifier + ' ') + (char)(' ' + x + 1) + (char)(' ' + y + 1);

                LastMousePosition.Set(x, y);

                SendData.Invoke(this,
                    new SendDataEventArgs
                    {
                        Data = Utf8MouseMode ?
                            Encoding.UTF8.GetBytes(x11Message) :
                            x11Message.Select(s => (byte)(Math.Min(0xFF, (int)s))).ToArray()
                    }
                );
            }
        }

        /// <summary>
        /// When appropriate for the given mode transmits a got focus message
        /// </summary>
        public void FocusIn()
        {
            if (SendData == null)
                return;

            if (SendFocusInAndFocusOutEvents)
            {
                var message = "\u001b[I";

                SendData.Invoke(this,
                    new SendDataEventArgs
                    {
                        Data = Encoding.UTF8.GetBytes(message)
                    }
                );
            }
        }

        /// <summary>
        /// When appropriate for the given mode transmits a lost focus message
        /// </summary>
        public void FocusOut()
        {
            if (SendData == null)
                return;

            if (SendFocusInAndFocusOutEvents)
            {
                var message = "\u001b[O";

                SendData.Invoke(this,
                    new SendDataEventArgs
                    {
                        Data = Encoding.UTF8.GetBytes(message)
                    }
                );
            }
        }

        public void PopXTermWindowIcon()
        {
            LogController($"(Not implemented) Restore xterm icon from stack.");
        }

        public void PopXTermWindowTitle()
        {
            LogController($"(Not implemented) Restore xterm window title from stack.");
        }

        public void PushXTermWindowIcon()
        {
            LogController($"(Not implemented) Save xterm icon on stack.");
        }

        public void PushXTermWindowTitle()
        {
            LogController($"(Not implemented) Save xterm window title on stack.");
        }

        public void XTermDeiconifyWindow()
        {
            LogController("(Not implemented) De-iconify window");
        }

        public void XTermFullScreenEnter()
        {
            LogController($"(Not implemented) Change to full-screen mode.");
        }
        public void XTermFullScreenExit()
        {
            LogController($"(Not implemented) Undo full-screen mode.");
        }
        public void XTermFullScreenToggle()
        {
            LogController($"(Not implemented) Toggle full-screen mode.");
        }

        public void XTermIconifyWindow()
        {
            LogController("(Not implemented) Iconify window");
        }

        public void XTermLowerToBottom()
        {
            LogController($"(Not implemented) Lower xterm window to bottom of the stacking order");
        }

        public void XTermMaximizeWindow(bool horizontally, bool vertically)
        {
            if (!horizontally && !vertically)
                LogController("(Not implemented) XTerm Restore maximized window");
            else
                LogController($"(Not implemented) XTerm maxmimize window horizontally={horizontally}, vertically={vertically}");
        }

        public void XTermMoveWindow(int x, int y)
        {
            LogController($"(Not implemented) Move windows to x={x},y={y}");
        }

        public void XTermRaiseToFront()
        {
            LogController($"(Not implemented) Raise xterm window to front of the stacking order");
        }

        public void XTermReport(XTermReportType reportType)
        {
            switch(reportType)
            {
                case XTermReportType.WindowState:
                    LogController($"(Not implemented) Report xterm window state.");
                    break;
                case XTermReportType.WindowPosition:
                    LogController($"(Not implemented) Report xterm window position????");
                    break;
                case XTermReportType.TextAreaPixelSize:
                    LogController($"(Not implemented) Report xterm text area size in pixels????");
                    break;
                case XTermReportType.ScreenPixelSize:
                    LogController($"(Not implemented) Report size of the screen in pixels.");
                    break;
                case XTermReportType.CharacterPixelSize:
                    LogController($"(Not implemented) Report xterm character size in pixels.");
                    break;
                case XTermReportType.TextAreaCharSize:
                    LogController($"(Not implemented) Report the size of the text area in characters.");
                    break;
                case XTermReportType.ScreenCharSize:
                    LogController($"(Not implemented) Report the size of the screen in characters.");
                    break;
                case XTermReportType.WindowIconLabel:
                    LogController($"(Not implemented) Report xterm window's icon label.");
                    break;
                case XTermReportType.WindowTitle:
                    LogController($"(Not implemented) Report xterm window's title.");
                    break;
                default:
                    LogController($"Unknown XTerm report type {reportType}");
                    break;
            }
        }

        public void XTermRefreshWindow()
        {
            LogController($"(Not implemented) Refresh xterm window");
        }

        public void XTermResizeTextArea(int columns, int rows)
        {
            LogController($"(Not implemented) Resize text area to columns=${columns}, rows={rows}");
        }

        public void XTermResizeWindow(int width, int height)
        {
            LogController($"(Not implemented) Resize xterm window to h={height}, w={width}");
        }

        public void ReportRGBBackgroundColor()
        {
            System.Diagnostics.Debug.WriteLine($"(Not implemented) OSC Get RGB background color");

            var report = "\u001b]11;" + CursorState.Attributes.BackgroundXParseColor + "\u0007";

            SendData?.Invoke(this, new SendDataEventArgs { Data = Encoding.UTF8.GetBytes(report) });
        }

        public void ReportRGBForegroundColor()
        {
            System.Diagnostics.Debug.WriteLine($"(Not implemented) OSC Get RGB foreground color");

            var report = "\u001b]10;" + CursorState.Attributes.XParseColor + "\u0007";

            SendData?.Invoke(this, new SendDataEventArgs { Data = Encoding.UTF8.GetBytes(report) });
        }
    }
}
