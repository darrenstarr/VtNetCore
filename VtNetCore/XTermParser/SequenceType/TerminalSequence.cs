namespace VtNetCore.XTermParser.SequenceType
{
    using System.Collections.Generic;
    using System.Linq;

    public class TerminalSequence
    {
        public List<int> Parameters { get; set; }
        public bool IsQuery { get; set; }
        public bool IsSend { get; set; }
        public bool IsBang { get; set; }
        public string Command { get; set; }
        public List<TerminalSequence> ProcessFirst { get; set; }

        public override string ToString()
        {
            return
                "(" +
                (IsQuery ? "?," : "") +
                (IsSend ? ">," : "") +
                (IsBang ? "!," : "") +
                ((Parameters != null && Parameters.Count > 0) ?
                    "[" + string.Join(",", Parameters.Select(x => x.ToString())) + "]" : ""
                ) +
                "'" + Command + "'" +
                "(" + string.Join(".",Command.Select(x => ((int)x).ToString("X2"))) + ")" + 
                ")";
        }
    }
}
