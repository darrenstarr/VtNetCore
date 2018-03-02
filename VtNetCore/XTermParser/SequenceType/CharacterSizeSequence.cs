namespace VtNetCore.XTermParser.SequenceType
{
    using VtNetCore.VirtualTerminal.Enums;

    public class CharacterSizeSequence : TerminalSequence
    {
        public ECharacterSize Size { get; set; }
        public override string ToString()
        {
            return "Character size - " + Size.ToString();
        }
    }
}
