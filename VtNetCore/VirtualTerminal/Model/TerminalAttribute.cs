namespace VtNetCore.VirtualTerminal.Model
{
    using VtNetCore.VirtualTerminal.Enums;

    /// <summary>
    /// Abstracts VT terminal character attributes
    /// </summary>
    public class TerminalAttribute
    {
        private static readonly ushort BrightBit = 0x0040;
        private static readonly ushort UnderscoreBit = 0x0080;
        private static readonly ushort StandoutBit = 0x0100;
        private static readonly ushort BlinkBit = 0x0200;
        private static readonly ushort ReverseBit = 0x0400;
        private static readonly ushort HiddenBit = 0x0800;
        private static readonly ushort ProtectionBit = 0x1000;

        private ushort InternalBits =
            (ushort)ETerminalColor.White |     // ForegroundColor
            (ushort)ETerminalColor.Black;      // BackgroundColor

        public TerminalColor ForegroundRgb { get; set; }

        public TerminalColor BackgroundRgb { get; set; }

        /// <summary>
        /// The foreground color of the text
        /// </summary>
        public ETerminalColor ForegroundColor
        {
            get
            {
                return (ETerminalColor)(InternalBits & 0x7);
            }
            set
            {
                InternalBits = (ushort)((InternalBits & 0xFFF8) | (ushort)value);
            }
        }


        /// <summary>
        /// The background color of the text
        /// </summary>
        public ETerminalColor BackgroundColor
        {
            get
            {
                return (ETerminalColor)((InternalBits >> 3) & 0x7);
            }
            set
            {
                InternalBits = (ushort)((InternalBits & 0xFFC7) | ((int)value << 3));
            }
        }

        /// <summary>
        /// Sets the text as bold.
        /// </summary>
        /// <remarks>
        /// This is an old naming system and should be udpated
        /// </remarks>
        public bool Bright
        {
            get
            {
                return (InternalBits & BrightBit) == BrightBit;
            }
            set
            {
                if (value)
                    InternalBits |= BrightBit;
                else
                    InternalBits = (ushort)((InternalBits & ~BrightBit) & 0xFFFF);
            }
        }

        /// <summary>
        /// Unclear what this is
        /// </summary>
        /// TODO : Figure out what Standout text is
        public bool Standout
        {
            get
            {
                return (InternalBits & StandoutBit) == StandoutBit;
            }
            set
            {
                if (value)
                    InternalBits |= StandoutBit;
                else
                    InternalBits = (ushort)((InternalBits & ~StandoutBit) & 0xFFFF);
            }
        }

        /// <summary>
        /// Sets the text as having a line beneath the character
        /// </summary>
        public bool Underscore
        {
            get
            {
                return (InternalBits & UnderscoreBit) == UnderscoreBit;
            }
            set
            {
                if (value)
                    InternalBits |= UnderscoreBit;
                else
                    InternalBits = (ushort)((InternalBits & ~UnderscoreBit) & 0xFFFF);
            }
        }

        /// <summary>
        /// Sets the blink attribute
        /// </summary>
        public bool Blink
        {
            get
            {
                return (InternalBits & BlinkBit) == BlinkBit;
            }
            set
            {
                if (value)
                    InternalBits |= BlinkBit;
                else
                    InternalBits = (ushort)((InternalBits & ~BlinkBit) & 0xFFFF);
            }
        }

        /// <summary>
        /// Reverses the foreground and the background colors of the text
        /// </summary>
        public bool Reverse
        {
            get
            {
                return (InternalBits & ReverseBit) == ReverseBit;
            }
            set
            {
                if (value)
                    InternalBits |= ReverseBit;
                else
                    InternalBits = (ushort)((InternalBits & ~ReverseBit) & 0xFFFF);
            }
        }

        /// <summary>
        /// Specifies that the character should not be displayed.
        /// </summary>
        public bool Hidden
        {
            get
            {
                return (InternalBits & HiddenBit) == HiddenBit;
            }
            set
            {
                if (value)
                    InternalBits |= HiddenBit;
                else
                    InternalBits = (ushort)((InternalBits & ~HiddenBit) & 0xFFFF);
            }
        }

        /// <summary>
        /// Specifies that the character should not be erased on SEL and SED operations.
        /// </summary>
        public bool Protected
        {
            get
            {
                return (InternalBits & ProtectionBit) == ProtectionBit;
            }
            set
            {
                if (value)
                    InternalBits |= ProtectionBit;
                else
                    InternalBits = (ushort)((InternalBits & ~ProtectionBit) & 0xFFFF);
            }
        }

        /// <summary>
        /// Returns a deep copy of the attribute
        /// </summary>
        /// <returns>A deep copy operation</returns>
        public TerminalAttribute Clone()
        {
            return new TerminalAttribute
            {
                InternalBits = InternalBits,
                ForegroundRgb = ForegroundRgb == null ? null : new TerminalColor(ForegroundRgb),
                BackgroundRgb = BackgroundRgb == null ? null : new TerminalColor(BackgroundRgb)
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
                "  ForegroundRgb: " + ForegroundRgb == null ? "<null>" : ForegroundRgb.ToString() + "\n" + 
                "  BackgroundColor: " + BackgroundColor.ToString() + "\n" +
                "  BackgroundRgb: " + BackgroundRgb == null ? "<null>" : BackgroundRgb.ToString() + "\n" +
                "  Bright: " + Bright.ToString() + "\n" +
                "  Standout: " + Standout.ToString() + "\n" +
                "  Underscore: " + Underscore.ToString() + "\n" +
                "  Blink: " + Blink.ToString() + "\n" +
                "  Reverse: " + Reverse.ToString() + "\n" +
                "  Hidden: " + Hidden.ToString() + "\n" + 
                "  Protected: " + Protected.ToString() + "\n"
                ;
        }
    }
}
