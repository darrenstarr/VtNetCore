namespace VtNetCore.XTermParser.SequenceType
{
    public class CharacterSequence : TerminalSequence
    {
        public char Character { get; set; }
        public override string ToString()
        {
            return "Character - '" + Character.ToString() + "' [" + ((int)Character).ToString("X2") + "]";
        }
    }
}
