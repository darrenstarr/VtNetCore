namespace VtNetCore.XTermParser
{
    using System;
    using VtNetCore.VirtualTerminal;
    using VtNetCore.XTermParser.SequenceType;

    public class SequenceHandler
    {
        public enum ESequenceType
        {
            Character,
            CSI,            // Control Sequence Introducer
            OSC,            // Operating System Command
            DCS,            // Device Control String
            SS3,            // Signal Shift Select 3
            VT52mc,         // VT52 Move Cursor
            Compliance,     // Compliance
            CharacterSet,   // Character set
            Escape,
            CharacterSize,
            Unicode
        }

        public enum Vt52Mode
        {
            Irrelevent,
            Yes,
            No
        }

        public string Description { get; set; }
        public ESequenceType SequenceType { get; set; }
        public int ExactParameterCount { get; set; } = -1;
        public int ExactParameterCountOrDefault { get; set; } = -1;
        public int DefaultParamValue { get; set; } = 1;
        public int MinimumParameterCount { get; set; } = 0;
        public bool Query { get; set; } = false;
        public bool Send { get; set; } = false;
        public bool Bang { get; set; } = false;
        public int[] Param0 { get; set; } = new int[] { };
        public int[] ValidParams { get; set; } = new int[] { };
        public string CsiCommand { get; set; }
        public string OscText { get; set; }
        public Action<TerminalSequence, IVirtualTerminalController> Handler { get; set; }
        public Vt52Mode Vt52 { get; set; } = Vt52Mode.Irrelevent;
    }
}
