namespace VtNetCore.VirtualTerminal.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using VtNetCore.VirtualTerminal.Enums;

    /// <summary>
    /// Abstracts VT terminal character attributes
    /// </summary>
    public class TerminalAttribute
    {
        public TerminalColor ForegroundRgb { get; set; } = TerminalColorsLookup[ETerminalColor.Black];

        public TerminalColor BackgroundRgb { get; set; } = TerminalColorsLookup[ETerminalColor.White];

        public static readonly Dictionary<ETerminalColor, TerminalColor> TerminalColorsLookup = new Dictionary<ETerminalColor, TerminalColor>()
        {
            { ETerminalColor.Black, new TerminalColor(ETerminalColor.Black, false) },
            { ETerminalColor.Red, new TerminalColor(ETerminalColor.Red, false)  },
            { ETerminalColor.Green, new TerminalColor(ETerminalColor.Green, false) },
            { ETerminalColor.Yellow,new TerminalColor(ETerminalColor.Yellow, false)  },
            { ETerminalColor.Blue, new TerminalColor(ETerminalColor.Blue, false) },
            { ETerminalColor.Magenta, new TerminalColor(ETerminalColor.Magenta, false) },
            { ETerminalColor.Cyan, new TerminalColor(ETerminalColor.Cyan, false) },
            { ETerminalColor.White, new TerminalColor(ETerminalColor.White, true) }
        };

        public override bool Equals(object obj)
        {
            if (this == null && obj == null)
                return true;

            if (this == null || obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as TerminalAttribute;

            if (other == null)
                return false;

            if (
                (ForegroundRgb == null && other.ForegroundRgb != null) ||
                (ForegroundRgb != null && other.ForegroundRgb == null) ||
                (
                    (ForegroundRgb != null && other.ForegroundRgb != null) &&
                    !ForegroundRgb.Equals(other.ForegroundRgb)
                )
            )
                return false;

            if (
                (BackgroundRgb == null && other.BackgroundRgb != null) ||
                (BackgroundRgb != null && other.BackgroundRgb == null) ||
                (
                    (BackgroundRgb != null && other.BackgroundRgb != null) &&
                    !BackgroundRgb.Equals(other.BackgroundRgb)
                )
            )
                return false;

            return Bright == other.Bright &&
                Standout == other.Standout &&
                Underscore == other.Underscore &&
                Blink == other.Blink &&
                Reverse == other.Reverse;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// The foreground color of the text
        /// </summary>
        public ETerminalColor ForegroundColor
        {
            get
            {
                return TerminalColorsLookup.Single(x => x.Value == ForegroundRgb).Key;
            }
            set
            {
                ForegroundRgb = TerminalColorsLookup[value];
            }
        }


        /// <summary>
        /// The background color of the text
        /// </summary>
        public ETerminalColor BackgroundColor
        {
            get
            {
                return TerminalColorsLookup.Single(x => x.Value == BackgroundRgb).Key;
            }
            set
            {
                BackgroundRgb = TerminalColorsLookup[value];
            }
        }

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
        /// Specifies that the character should not be erased on SEL and SED operations.
        /// </summary>
        public int Protected { get; set; } = 0;

        /// <summary>
        /// Returns a deep copy of the attribute
        /// </summary>
        /// <returns>A deep copy operation</returns>
        public TerminalAttribute Clone()
        {
            return new TerminalAttribute
            {
                ForegroundRgb = ForegroundRgb == null ? null : new TerminalColor(ForegroundRgb),
                BackgroundRgb = BackgroundRgb == null ? null : new TerminalColor(BackgroundRgb),
                Bright = Bright,
                Standout = Standout,
                Underscore = Underscore,
                Blink = Blink,
                Reverse = Reverse,
                Hidden = Hidden,
                Protected = Protected
            };
        }

        /// <summary>
        /// Returns a deep copy of the attribute with reversed colors
        /// </summary>
        /// <returns>A deep copy operation</returns>
        public TerminalAttribute Inverse
        {
            get
            {
                return new TerminalAttribute
                {
                    ForegroundRgb = BackgroundRgb == null ? null : new TerminalColor(BackgroundRgb),
                    BackgroundRgb = ForegroundRgb == null ? null : new TerminalColor(ForegroundRgb),
                    Bright = Bright,
                    Standout = Standout,
                    Underscore = Underscore,
                    Blink = Blink,
                    Reverse = Reverse,
                    Hidden = Hidden,
                    Protected = Protected
                };
            }
        }

        /// <summary>
        /// Returns the foreground color as a web color
        /// </summary>
        public string WebColor
        {
            get
            {
                if (ForegroundRgb != null)
                    return ForegroundRgb.WebColor;

                return (new TerminalColor(ForegroundColor, Bright)).WebColor;
            }
        }

        /// <summary>
        /// Returns the foreground color as a XParseColor string
        /// </summary>
        public string XParseColor
        {
            get
            {
                if (ForegroundRgb != null)
                    return ForegroundRgb.XParseColor;

                return (new TerminalColor(ForegroundColor, Bright)).XParseColor;
            }
        }

        /// <summary>
        /// Returns the background color as a web color
        /// </summary>
        public string BackgroundWebColor
        {
            get
            {
                if (BackgroundRgb != null)
                    return BackgroundRgb.WebColor;

                return (new TerminalColor(BackgroundColor, false)).WebColor;
            }
        }

        /// <summary>
        /// Returns the background color as a XParseColor string
        /// </summary>
        public string BackgroundXParseColor
        {
            get
            {
                if (BackgroundRgb != null)
                    return BackgroundRgb.XParseColor;

                return (new TerminalColor(BackgroundColor, false)).XParseColor;
            }
        }

        /// <summary>
        /// Returns a verbose indented string for debugging
        /// </summary>
        /// <returns>a debugging string</returns>
        public override string ToString()
        {
            return
                "  ForegroundColor: " + ForegroundColor.ToString() + "\n" +
                "  ForegroundRgb: " + (ForegroundRgb == null ? "<null>" : ForegroundRgb.ToString()) + "\n" + 
                "  BackgroundColor: " + BackgroundColor.ToString() + "\n" +
                "  BackgroundRgb: " + (BackgroundRgb == null ? "<null>" : BackgroundRgb.ToString()) + "\n" +
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
