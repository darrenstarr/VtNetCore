namespace VtNetCore.VirtualTerminal
{
    using VtNetCore.VirtualTerminal.Model;

    public class VirtualTerminalViewPort
    {
        public VirtualTerminalController Parent { get; private set; }

        public int TopRow { get { return Parent.TopRow; } }

        internal VirtualTerminalViewPort(VirtualTerminalController controller)
        {
            Parent = controller;
        }

        public TerminalLine [] GetLines(int from, int count)
        {
            var result = new TerminalLine[count];

            for(int i=0; i<count; i++)
                result[i] = ((from + i) >= Parent.Buffer.Count) ? null : Parent.Buffer[from + i];

            return result;
        }

        public TerminalLine GetLine(int row)
        {
            return ((row) >= Parent.Buffer.Count) ? null : Parent.Buffer[row]; 
        }

        public TerminalLine GetVisibleLine(int row)
        {
            return GetLine(row + TopRow);
        }

        public void SetTopLine(int top)
        {
            Parent.TopRow = top;
        }

        /// <summary>
        /// Returns the cursor position in terms of base-1 relative to the view port
        /// </summary>
        public TextPosition ScreenCursorPosition
        {
            get
            {
                return CursorPosition.OffsetBy(1, 1);
            }
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
    }
}
