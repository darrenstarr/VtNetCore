namespace VtNetCore.XTermParser.SequenceType
{
    public class EscapeSequence : TerminalSequence
    {
        public override string ToString()
        {
            return "ESC - " + base.ToString();
        }
    }
}
