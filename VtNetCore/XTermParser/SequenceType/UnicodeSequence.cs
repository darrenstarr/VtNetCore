namespace VtNetCore.XTermParser.SequenceType
{
    public class UnicodeSequence : EscapeSequence
    {
        public override string ToString()
        {
            return "Unicode - " + base.ToString();
        }
    }
}
