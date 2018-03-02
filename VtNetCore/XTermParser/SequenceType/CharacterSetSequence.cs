namespace VtNetCore.XTermParser.SequenceType
{
    using VtNetCore.VirtualTerminal.Enums;

    public class CharacterSetSequence : TerminalSequence
    {
        public ECharacterSet CharacterSet { get; set; }
        public ECharacterSetMode Mode { get; set; }
        public override string ToString()
        {
            return "Character set - " + Mode.ToString() + " is " + CharacterSet.ToString();
        }
    }
}
