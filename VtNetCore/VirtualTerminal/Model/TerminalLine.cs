namespace VtNetCore.VirtualTerminal.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// A single line of text within the terminal
    /// </summary>
    public class TerminalLine : List<TerminalCharacter>
    {
        /// <summary>
        /// Specifies that the text on this line should be scaled (and spaced) by a factor of two
        /// </summary>
        public bool DoubleWidth { get; set; } = false;
        
        /// <summary>
        /// Specifies that the text on this line should contain the top half of a double high/double wide/double spaced character
        /// </summary>
        public bool DoubleHeightTop { get; set; } = false;

        /// <summary>
        /// Specifies that the text on this line should contain the bottom half of a double high/double wide/double spaced character
        /// </summary>
        public bool DoubleHeightBottom { get; set; } = false;
    }
}
