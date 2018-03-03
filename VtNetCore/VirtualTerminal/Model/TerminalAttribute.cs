namespace VtNetCore.VirtualTerminal.Model
{
    using VtNetCore.VirtualTerminal.Enums;

    /// <summary>
    /// Abstracts VT terminal character attributes
    /// </summary>
    public class TerminalAttribute
    {
        /// <summary>
        /// The foreground color of the text
        /// </summary>
        public ETerminalColor ForegroundColor { get; set; } = ETerminalColor.White;
        /// <summary>
        /// The background color of the text
        /// </summary>
        public ETerminalColor BackgroundColor { get; set; } = ETerminalColor.Black;
        /// <summary>
        /// Sets the text as bold.
        /// </summary>
        /// <remarks>
        /// This is an old naming system and should be udpated
        /// </remarks>
        public bool Bright { get; set; } = false;
        /// <summary>
        /// Unclear what this is
        /// </summary>
        /// TODO : Figure out what Standout text is
        public bool Standout { get; set; } = false;
        /// <summary>
        /// Sets the text as having a line beneath the character
        /// </summary>
        public bool Underscore { get; set; } = false;
        /// <summary>
        /// Sets the blink attribute
        /// </summary>
        public bool Blink { get; set; } = false;
        /// <summary>
        /// Reverses the foreground and the background colors of the text
        /// </summary>
        public bool Reverse { get; set; } = false;
        /// <summary>
        /// Specifies that the character should not be displayed.
        /// </summary>
        public bool Hidden { get; set; } = false;

        /// <summary>
        /// Returns a deep copy of the attribute
        /// </summary>
        /// <returns>A deep copy operation</returns>
        public TerminalAttribute Clone()
        {
            return new TerminalAttribute
            {
                ForegroundColor = ForegroundColor,
                BackgroundColor = BackgroundColor,
                Bright = Bright,
                Standout = Standout,
                Underscore = Underscore,
                Blink = Blink,
                Reverse = Reverse,
                Hidden = Hidden
            };
        }

        /// <summary>
        /// Returns a verbose indented string for debugging
        /// </summary>
        /// <returns>a debugging string</returns>
        public override string ToString()
        {
            return
                "  ForegroundColor: " + ForegroundColor.ToString() + "\n" +
                "  BackgroundColor: " + BackgroundColor.ToString() + "\n" +
                "  Bright: " + Bright.ToString() + "\n" +
                "  Standout: " + Standout.ToString() + "\n" +
                "  Underscore: " + Underscore.ToString() + "\n" +
                "  Blink: " + Blink.ToString() + "\n" +
                "  Reverse: " + Reverse.ToString() + "\n" +
                "  Hidden: " + Hidden.ToString()
                ;
        }
    }
}
