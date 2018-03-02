namespace VtNetCore.XTermParser.SequenceType
{
    public class CsiSequence : TerminalSequence
    {
        public override string ToString()
        {
            return "CSI - " + base.ToString();
        }
    }
}
