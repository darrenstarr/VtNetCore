namespace VtNetCore.VirtualTerminal
{
    /// <summary>
    /// Specifies the character sequences to transmit for keypresses
    /// </summary>
    internal class KeyboardTranslation
    {
        /// <summary>
        /// Sequence to transmit when neither shift or control are pressed
        /// </summary>
        public string Normal { get; set; }

        /// <summary>
        /// Sequence to transmit when neither shift is pressed
        /// </summary>
        public string Shift { get; set; }

        /// <summary>
        /// Sequence to transmit when neither control is pressed
        /// </summary>
        public string Control { get; set; }

        /// <summary>
        /// Specifies that the shift sequence should be sent regardless of control or shift in application mode
        /// </summary>
        public bool ShiftOnApplication { get; set; }

    }
}
