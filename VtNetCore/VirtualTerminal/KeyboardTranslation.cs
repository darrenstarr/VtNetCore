namespace VtNetCore.VirtualTerminal
{
    /// <summary>
    /// Specifies the character sequences to transmit for keypresses
    /// </summary>
    internal class KeyboardTranslation
    {
        /// <summary>
        /// Sequence to transmit when operating in "Normal Mode"
        /// </summary>
        public byte [] NormalMode { get; set; }

        /// <summary>
        /// Sequence to transmit when operating in "Application Mode"
        /// </summary>
        public byte [] ApplicationMode { get; set; }
    }
}
