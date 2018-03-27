namespace VtNetCore.VirtualTerminal.Layout
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class LayoutRow
    {
        public int LogicalRowNumber { get; set; }
        public bool DoubleWidth { get; set; }
        public bool DoubleHeightTop { get; set; }
        public bool DoubleHeightBottom { get; set; }
        public List<LayoutSpan> Spans { get; set; } = new List<LayoutSpan>();
    }
}
