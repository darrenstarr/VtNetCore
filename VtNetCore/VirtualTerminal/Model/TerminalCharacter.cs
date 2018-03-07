namespace VtNetCore.VirtualTerminal.Model
{
    /// <summary>
    /// Represents a single character and its attributes
    /// </summary>
    public class TerminalCharacter
    {
        /// <summary>
        /// The character to display
        /// </summary>
        public char Char { get; set; } = ' ';

        /// <summary>
        /// Specifies the lower portion of a combining character
        /// </summary>
        public string CombiningCharacters { get; set; }

        /// <summary>
        /// The attributes to apply to the character
        /// </summary>
        public TerminalAttribute Attributes { get; set; } = new TerminalAttribute();

        /// <summary>
        /// Protected characters can't be erased
        /// </summary>
        public bool Protected { get; set; }

        /// <summary>
        /// Deep copy/clone
        /// </summary>
        /// <returns>A deep copy of this object</returns>
        /// TODO : Implement ICloneable
        public TerminalCharacter Clone()
        {
            return new TerminalCharacter
            {
                Char = Char,
                CombiningCharacters = CombiningCharacters,
                Attributes = Attributes.Clone(),
            };
        }
    }
}
