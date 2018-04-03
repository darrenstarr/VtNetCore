using System;
using System.Collections.Generic;
using System.Text;

namespace VtNetCore.VirtualTerminal
{
    public class SizeEventArgs : EventArgs
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
