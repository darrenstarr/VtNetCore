namespace VtNetCore.VirtualTerminal
{
    using System.Collections.Generic;
    using VtNetCore.VirtualTerminal.Model;

    /// <summary>
    /// 
    /// </summary>
    public class VirtualTerminalViewPort
    {
        public VirtualTerminalController Parent { get; private set; }

        /// <summary>
        /// The current active top row of the buffer.
        /// </summary>
        /// <remarks>
        /// This describes where the terminal believes the top of the screen is.
        /// </remarks>
        public int TopRow { get { return Parent.TopRow; } }

        internal VirtualTerminalViewPort(VirtualTerminalController controller)
        {
            Parent = controller;
        }

        /// <summary>
        /// Returns a list of lines from the terminal model buffer
        /// </summary>
        /// <param name="from">The first line to return</param>
        /// <param name="count">The number of lines to return as a liat</param>
        /// <returns>The requetsed lines</returns>
        public TerminalLine [] GetLines(int from, int count)
        {
            var result = new TerminalLine[count];

            for(int i=0; i<count; i++)
                result[i] = ((from + i) >= Parent.Buffer.Count) ? null : Parent.Buffer[from + i];

            return result;
        }

        /// <summary>
        /// Returns the model for the given line of the terminal
        /// </summary>
        /// <param name="row">the base-zero inde of the requested line</param>
        /// <returns>The requested line or null if the line did not yet exist.</returns>
        public TerminalLine GetLine(int row)
        {
            return ((row) >= Parent.Buffer.Count) ? null : Parent.Buffer[row]; 
        }

        /// <summary>
        /// Gets a line relative to the top of the logical screen
        /// </summary>
        /// <param name="row">The requested row number (zero based)</param>
        /// <returns>The requested line or null if it does not exist</returns>
        public TerminalLine GetVisibleLine(int row)
        {
            return GetLine(row + TopRow);
        }

        /// <summary>
        /// Returns the cursor position in terms of base-1 relative to the view port
        /// </summary>
        public TextPosition ScreenCursorPosition
        {
            get { return CursorPosition.OffsetBy(1, 1); }
        }

        /// <summary>
        /// Returns the cursor position in terms of base-0 relative to the view port
        /// </summary>
        public TextPosition CursorPosition
        {
            get
            {
                var column = Parent.CursorState.CurrentColumn;
                if (Parent.CursorState.WordWrap && column >= Parent.Columns)
                    column = Parent.Columns - 1;

                return new TextPosition
                {
                    Column = column,
                    Row = Parent.CursorState.CurrentRow
                };
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
        public List<Layout.LayoutRow> GetPageSpans(int startingLine, int lineCount, int width = -1, TextRange invertedRange = null)
        {
            return Parent.GetPageSpans(startingLine, lineCount, width, invertedRange);
        }
    }
}
