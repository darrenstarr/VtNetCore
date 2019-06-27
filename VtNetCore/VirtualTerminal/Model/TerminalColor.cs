using System;
using System.Collections.Generic;
using System.Text;
using VtNetCore.VirtualTerminal.Enums;

namespace VtNetCore.VirtualTerminal.Model
{
    public class TerminalColor
    {
        public uint ARGB { get; set; }

        public TerminalColor()
        {
        }

        public TerminalColor(TerminalColor other)
        {
            ARGB = other.ARGB;
        }

        public TerminalColor(string webColor)
        {
            Red = Convert.ToUInt32(webColor.Substring(1, 2), 16);
            Green = Convert.ToUInt32(webColor.Substring(3, 2), 16);
            Blue = Convert.ToUInt32(webColor.Substring(5, 2), 16);
        }

        public TerminalColor(ETerminalColor color, bool bright)
        {
            Set(color, bright);
        }

        public override bool Equals(object obj)
        {
            if (this == null && obj == null)
                return true;

            if (this == null || obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as TerminalColor;

            return ARGB == other.ARGB;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return WebColor;
        }

        public static readonly Dictionary<int, TerminalColor> Iso8613 = new Dictionary<int, TerminalColor>
        {
            {  16, new TerminalColor("#000000") }, // Grey0
            {  17, new TerminalColor("#00005f") }, // NavyBlue
            {  18, new TerminalColor("#000087") }, // DarkBlue
            {  19, new TerminalColor("#0000af") }, // Blue3
            {  20, new TerminalColor("#0000d7") }, // Blue3
            {  21, new TerminalColor("#0000ff") }, // Blue1
            {  22, new TerminalColor("#005f00") }, // DarkGreen
            {  23, new TerminalColor("#005f5f") }, // DeepSkyBlue4
            {  24, new TerminalColor("#005f87") }, // DeepSkyBlue4
            {  25, new TerminalColor("#005faf") }, // DeepSkyBlue4
            {  26, new TerminalColor("#005fd7") }, // DodgerBlue3
            {  27, new TerminalColor("#005fff") }, // DodgerBlue2
            {  28, new TerminalColor("#008700") }, // Green4
            {  29, new TerminalColor("#00875f") }, // SpringGreen4
            {  30, new TerminalColor("#008787") }, // Turquoise4
            {  31, new TerminalColor("#0087af") }, // DeepSkyBlue3
            {  32, new TerminalColor("#0087d7") }, // DeepSkyBlue3
            {  33, new TerminalColor("#0087ff") }, // DodgerBlue1
            {  34, new TerminalColor("#00af00") }, // Green3
            {  35, new TerminalColor("#00af5f") }, // SpringGreen3
            {  36, new TerminalColor("#00af87") }, // DarkCyan
            {  37, new TerminalColor("#00afaf") }, // LightSeaGreen
            {  38, new TerminalColor("#00afd7") }, // DeepSkyBlue2
            {  39, new TerminalColor("#00afff") }, // DeepSkyBlue1
            {  40, new TerminalColor("#00d700") }, // Green3
            {  41, new TerminalColor("#00d75f") }, // SpringGreen3
            {  42, new TerminalColor("#00d787") }, // SpringGreen2
            {  43, new TerminalColor("#00d7af") }, // Cyan3
            {  44, new TerminalColor("#00d7d7") }, // DarkTurquoise
            {  45, new TerminalColor("#00d7ff") }, // Turquoise2
            {  46, new TerminalColor("#00ff00") }, // Green1
            {  47, new TerminalColor("#00ff5f") }, // SpringGreen2
            {  48, new TerminalColor("#00ff87") }, // SpringGreen1
            {  49, new TerminalColor("#00ffaf") }, // MediumSpringGreen
            {  50, new TerminalColor("#00ffd7") }, // Cyan2
            {  51, new TerminalColor("#00ffff") }, // Cyan1
            {  52, new TerminalColor("#5f0000") }, // DarkRed
            {  53, new TerminalColor("#5f005f") }, // DeepPink4
            {  54, new TerminalColor("#5f0087") }, // Purple4
            {  55, new TerminalColor("#5f00af") }, // Purple4
            {  56, new TerminalColor("#5f00d7") }, // Purple3
            {  57, new TerminalColor("#5f00ff") }, // BlueViolet
            {  58, new TerminalColor("#5f5f00") }, // Orange4
            {  59, new TerminalColor("#5f5f5f") }, // Grey37
            {  60, new TerminalColor("#5f5f87") }, // MediumPurple4
            {  61, new TerminalColor("#5f5faf") }, // SlateBlue3
            {  62, new TerminalColor("#5f5fd7") }, // SlateBlue3
            {  63, new TerminalColor("#5f5fff") }, // RoyalBlue1
            {  64, new TerminalColor("#5f8700") }, // Chartreuse4
            {  65, new TerminalColor("#5f875f") }, // DarkSeaGreen4
            {  66, new TerminalColor("#5f8787") }, // PaleTurquoise4
            {  67, new TerminalColor("#5f87af") }, // SteelBlue
            {  68, new TerminalColor("#5f87d7") }, // SteelBlue3
            {  69, new TerminalColor("#5f87ff") }, // CornflowerBlue
            {  70, new TerminalColor("#5faf00") }, // Chartreuse3
            {  71, new TerminalColor("#5faf5f") }, // DarkSeaGreen4
            {  72, new TerminalColor("#5faf87") }, // CadetBlue
            {  73, new TerminalColor("#5fafaf") }, // CadetBlue
            {  74, new TerminalColor("#5fafd7") }, // SkyBlue3
            {  75, new TerminalColor("#5fafff") }, // SteelBlue1
            {  76, new TerminalColor("#5fd700") }, // Chartreuse3
            {  77, new TerminalColor("#5fd75f") }, // PaleGreen3
            {  78, new TerminalColor("#5fd787") }, // SeaGreen3
            {  79, new TerminalColor("#5fd7af") }, // Aquamarine3
            {  80, new TerminalColor("#5fd7d7") }, // MediumTurquoise
            {  81, new TerminalColor("#5fd7ff") }, // SteelBlue1
            {  82, new TerminalColor("#5fff00") }, // Chartreuse2
            {  83, new TerminalColor("#5fff5f") }, // SeaGreen2
            {  84, new TerminalColor("#5fff87") }, // SeaGreen1
            {  85, new TerminalColor("#5fffaf") }, // SeaGreen1
            {  86, new TerminalColor("#5fffd7") }, // Aquamarine1
            {  87, new TerminalColor("#5fffff") }, // DarkSlateGray2
            {  88, new TerminalColor("#870000") }, // DarkRed
            {  89, new TerminalColor("#87005f") }, // DeepPink4
            {  90, new TerminalColor("#870087") }, // DarkMagenta
            {  91, new TerminalColor("#8700af") }, // DarkMagenta
            {  92, new TerminalColor("#8700d7") }, // DarkViolet
            {  93, new TerminalColor("#8700ff") }, // Purple
            {  94, new TerminalColor("#875f00") }, // Orange4
            {  95, new TerminalColor("#875f5f") }, // LightPink4
            {  96, new TerminalColor("#875f87") }, // Plum4
            {  97, new TerminalColor("#875faf") }, // MediumPurple3
            {  98, new TerminalColor("#875fd7") }, // MediumPurple3
            {  99, new TerminalColor("#875fff") }, // SlateBlue1
            { 100, new TerminalColor("#878700") }, // Yellow4
            { 101, new TerminalColor("#87875f") }, // Wheat4
            { 102, new TerminalColor("#878787") }, // Grey53
            { 103, new TerminalColor("#8787af") }, // LightSlateGrey
            { 104, new TerminalColor("#8787d7") }, // MediumPurple
            { 105, new TerminalColor("#8787ff") }, // LightSlateBlue
            { 106, new TerminalColor("#87af00") }, // Yellow4
            { 107, new TerminalColor("#87af5f") }, // DarkOliveGreen3
            { 108, new TerminalColor("#87af87") }, // DarkSeaGreen
            { 109, new TerminalColor("#87afaf") }, // LightSkyBlue3
            { 110, new TerminalColor("#87afd7") }, // LightSkyBlue3
            { 111, new TerminalColor("#87afff") }, // SkyBlue2
            { 112, new TerminalColor("#87d700") }, // Chartreuse2
            { 113, new TerminalColor("#87d75f") }, // DarkOliveGreen3
            { 114, new TerminalColor("#87d787") }, // PaleGreen3
            { 115, new TerminalColor("#87d7af") }, // DarkSeaGreen3
            { 116, new TerminalColor("#87d7d7") }, // DarkSlateGray3
            { 117, new TerminalColor("#87d7ff") }, // SkyBlue1
            { 118, new TerminalColor("#87ff00") }, // Chartreuse1
            { 119, new TerminalColor("#87ff5f") }, // LightGreen
            { 120, new TerminalColor("#87ff87") }, // LightGreen
            { 121, new TerminalColor("#87ffaf") }, // PaleGreen1
            { 122, new TerminalColor("#87ffd7") }, // Aquamarine1
            { 123, new TerminalColor("#87ffff") }, // DarkSlateGray1
            { 124, new TerminalColor("#af0000") }, // Red3
            { 125, new TerminalColor("#af005f") }, // DeepPink4
            { 126, new TerminalColor("#af0087") }, // MediumVioletRed
            { 127, new TerminalColor("#af00af") }, // Magenta3
            { 128, new TerminalColor("#af00d7") }, // DarkViolet
            { 129, new TerminalColor("#af00ff") }, // Purple
            { 130, new TerminalColor("#af5f00") }, // DarkOrange3
            { 131, new TerminalColor("#af5f5f") }, // IndianRed
            { 132, new TerminalColor("#af5f87") }, // HotPink3
            { 133, new TerminalColor("#af5faf") }, // MediumOrchid3
            { 134, new TerminalColor("#af5fd7") }, // MediumOrchid
            { 135, new TerminalColor("#af5fff") }, // MediumPurple2
            { 136, new TerminalColor("#af8700") }, // DarkGoldenrod
            { 137, new TerminalColor("#af875f") }, // LightSalmon3
            { 138, new TerminalColor("#af8787") }, // RosyBrown
            { 139, new TerminalColor("#af87af") }, // Grey63
            { 140, new TerminalColor("#af87d7") }, // MediumPurple2
            { 141, new TerminalColor("#af87ff") }, // MediumPurple1
            { 142, new TerminalColor("#afaf00") }, // Gold3
            { 143, new TerminalColor("#afaf5f") }, // DarkKhaki
            { 144, new TerminalColor("#afaf87") }, // NavajoWhite3
            { 145, new TerminalColor("#afafaf") }, // Grey69
            { 146, new TerminalColor("#afafd7") }, // LightSteelBlue3
            { 147, new TerminalColor("#afafff") }, // LightSteelBlue
            { 148, new TerminalColor("#afd700") }, // Yellow3
            { 149, new TerminalColor("#afd75f") }, // DarkOliveGreen3
            { 150, new TerminalColor("#afd787") }, // DarkSeaGreen3
            { 151, new TerminalColor("#afd7af") }, // DarkSeaGreen2
            { 152, new TerminalColor("#afd7d7") }, // LightCyan3
            { 153, new TerminalColor("#afd7ff") }, // LightSkyBlue1
            { 154, new TerminalColor("#afff00") }, // GreenYellow
            { 155, new TerminalColor("#afff5f") }, // DarkOliveGreen2
            { 156, new TerminalColor("#afff87") }, // PaleGreen1
            { 157, new TerminalColor("#afffaf") }, // DarkSeaGreen2
            { 158, new TerminalColor("#afffd7") }, // DarkSeaGreen1
            { 159, new TerminalColor("#afffff") }, // PaleTurquoise1
            { 160, new TerminalColor("#d70000") }, // Red3
            { 161, new TerminalColor("#d7005f") }, // DeepPink3
            { 162, new TerminalColor("#d70087") }, // DeepPink3
            { 163, new TerminalColor("#d700af") }, // Magenta3
            { 164, new TerminalColor("#d700d7") }, // Magenta3
            { 165, new TerminalColor("#d700ff") }, // Magenta2
            { 166, new TerminalColor("#d75f00") }, // DarkOrange3
            { 167, new TerminalColor("#d75f5f") }, // IndianRed
            { 168, new TerminalColor("#d75f87") }, // HotPink3
            { 169, new TerminalColor("#d75faf") }, // HotPink2
            { 170, new TerminalColor("#d75fd7") }, // Orchid
            { 171, new TerminalColor("#d75fff") }, // MediumOrchid1
            { 172, new TerminalColor("#d78700") }, // Orange3
            { 173, new TerminalColor("#d7875f") }, // LightSalmon3
            { 174, new TerminalColor("#d78787") }, // LightPink3
            { 175, new TerminalColor("#d787af") }, // Pink3
            { 176, new TerminalColor("#d787d7") }, // Plum3
            { 177, new TerminalColor("#d787ff") }, // Violet
            { 178, new TerminalColor("#d7af00") }, // Gold3
            { 179, new TerminalColor("#d7af5f") }, // LightGoldenrod3
            { 180, new TerminalColor("#d7af87") }, // Tan
            { 181, new TerminalColor("#d7afaf") }, // MistyRose3
            { 182, new TerminalColor("#d7afd7") }, // Thistle3
            { 183, new TerminalColor("#d7afff") }, // Plum2
            { 184, new TerminalColor("#d7d700") }, // Yellow3
            { 185, new TerminalColor("#d7d75f") }, // Khaki3
            { 186, new TerminalColor("#d7d787") }, // LightGoldenrod2
            { 187, new TerminalColor("#d7d7af") }, // LightYellow3
            { 188, new TerminalColor("#d7d7d7") }, // Grey84
            { 189, new TerminalColor("#d7d7ff") }, // LightSteelBlue1
            { 190, new TerminalColor("#d7ff00") }, // Yellow2
            { 191, new TerminalColor("#d7ff5f") }, // DarkOliveGreen1
            { 192, new TerminalColor("#d7ff87") }, // DarkOliveGreen1
            { 193, new TerminalColor("#d7ffaf") }, // DarkSeaGreen1
            { 194, new TerminalColor("#d7ffd7") }, // Honeydew2
            { 195, new TerminalColor("#d7ffff") }, // LightCyan1
            { 196, new TerminalColor("#ff0000") }, // Red1
            { 197, new TerminalColor("#ff005f") }, // DeepPink2
            { 198, new TerminalColor("#ff0087") }, // DeepPink1
            { 199, new TerminalColor("#ff00af") }, // DeepPink1
            { 200, new TerminalColor("#ff00d7") }, // Magenta2
            { 201, new TerminalColor("#ff00ff") }, // Magenta1
            { 202, new TerminalColor("#ff5f00") }, // OrangeRed1
            { 203, new TerminalColor("#ff5f5f") }, // IndianRed1
            { 204, new TerminalColor("#ff5f87") }, // IndianRed1
            { 205, new TerminalColor("#ff5faf") }, // HotPink
            { 206, new TerminalColor("#ff5fd7") }, // HotPink
            { 207, new TerminalColor("#ff5fff") }, // MediumOrchid1
            { 208, new TerminalColor("#ff8700") }, // DarkOrange
            { 209, new TerminalColor("#ff875f") }, // Salmon1
            { 210, new TerminalColor("#ff8787") }, // LightCoral
            { 211, new TerminalColor("#ff87af") }, // PaleVioletRed1
            { 212, new TerminalColor("#ff87d7") }, // Orchid2
            { 213, new TerminalColor("#ff87ff") }, // Orchid1
            { 214, new TerminalColor("#ffaf00") }, // Orange1
            { 215, new TerminalColor("#ffaf5f") }, // SandyBrown
            { 216, new TerminalColor("#ffaf87") }, // LightSalmon1
            { 217, new TerminalColor("#ffafaf") }, // LightPink1
            { 218, new TerminalColor("#ffafd7") }, // Pink1
            { 219, new TerminalColor("#ffafff") }, // Plum1
            { 220, new TerminalColor("#ffd700") }, // Gold1
            { 221, new TerminalColor("#ffd75f") }, // LightGoldenrod2
            { 222, new TerminalColor("#ffd787") }, // LightGoldenrod2
            { 223, new TerminalColor("#ffd7af") }, // NavajoWhite1
            { 224, new TerminalColor("#ffd7d7") }, // MistyRose1
            { 225, new TerminalColor("#ffd7ff") }, // Thistle1
            { 226, new TerminalColor("#ffff00") }, // Yellow1
            { 227, new TerminalColor("#ffff5f") }, // LightGoldenrod1
            { 228, new TerminalColor("#ffff87") }, // Khaki1
            { 229, new TerminalColor("#ffffaf") }, // Wheat1
            { 230, new TerminalColor("#ffffd7") }, // Cornsilk1
            { 231, new TerminalColor("#ffffff") }, // Grey100
            { 232, new TerminalColor("#080808") }, // Grey3
            { 233, new TerminalColor("#121212") }, // Grey7
            { 234, new TerminalColor("#1c1c1c") }, // Grey11
            { 235, new TerminalColor("#262626") }, // Grey15
            { 236, new TerminalColor("#303030") }, // Grey19
            { 237, new TerminalColor("#3a3a3a") }, // Grey23
            { 238, new TerminalColor("#444444") }, // Grey27
            { 239, new TerminalColor("#4e4e4e") }, // Grey30
            { 240, new TerminalColor("#585858") }, // Grey35
            { 241, new TerminalColor("#626262") }, // Grey39
            { 242, new TerminalColor("#6c6c6c") }, // Grey42
            { 243, new TerminalColor("#767676") }, // Grey46
            { 244, new TerminalColor("#808080") }, // Grey50
            { 245, new TerminalColor("#8a8a8a") }, // Grey54
            { 246, new TerminalColor("#949494") }, // Grey58
            { 247, new TerminalColor("#9e9e9e") }, // Grey62
            { 248, new TerminalColor("#a8a8a8") }, // Grey66
            { 249, new TerminalColor("#b2b2b2") }, // Grey70
            { 250, new TerminalColor("#bcbcbc") }, // Grey74
            { 251, new TerminalColor("#c6c6c6") }, // Grey78
            { 252, new TerminalColor("#d0d0d0") }, // Grey82
            { 253, new TerminalColor("#dadada") }, // Grey85
            { 254, new TerminalColor("#e4e4e4") }, // Grey89
            { 255, new TerminalColor("#eeeeee") }, // Grey93
        };

        public uint Red
        {
            get
            {
                return (ARGB >> 16) & 0xFF;
            }
            set
            {
                ARGB = (ARGB & 0xFF00FFFF) | ((value & 0xFF) << 16);
            }
        }

        public uint Green
        {
            get
            {
                return (ARGB >> 8) & 0xFF;
            }
            set
            {
                ARGB = (ARGB & 0xFFFF00FF) | ((value & 0xFF) << 8);
            }
        }

        public uint Blue
        {
            get
            {
                return ARGB & 0xFF;
            }
            set
            {
                ARGB = (ARGB & 0xFFFFFF00) | (value & 0xFF);
            }
        }

        public string WebColor
        {
            get
            {
                return string.Format("#{0:X6}", ARGB);
            }
        }

        public string XParseColor
        {
            get
            {
                return string.Format("rgb:{0:X2}/{0:X2}/{0:X2}", Red, Green, Blue);
            }
        }

        public void Set(int paletteIndex)
        {
            if(Iso8613.TryGetValue(paletteIndex, out TerminalColor colorValue))
                ARGB = colorValue.ARGB;
            else
                ARGB = 0;
        }

        public void Set(uint red, uint green, uint blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public void Set(ETerminalColor termColor, bool bright)
        {
            if (bright)
            {
                switch (termColor)
                {
                    case ETerminalColor.Black:
                        Set(127, 127, 127);     // Bright black
                        break;

                    case ETerminalColor.Red:
                        Set(255, 0, 0);    // Bright red
                        break;

                    case ETerminalColor.Green:
                        Set(0, 255, 0);    // Bright green
                        break;

                    case ETerminalColor.Yellow:
                        Set(255, 255, 0);   // Bright yellow
                        break;

                    case ETerminalColor.Blue:
                        Set(92, 92, 255);    // Bright blue
                        break;

                    case ETerminalColor.Magenta:
                        Set(255, 0, 255);   // Bright Magenta
                        break;

                    case ETerminalColor.Cyan:
                        Set(0, 255, 255);   // Bright cyan
                        break;

                    case ETerminalColor.White:
                        Set(255, 255, 255);  // Bright white
                        break;
                }
            }
            else
            {
                switch (termColor)
                {
                    case ETerminalColor.Black:
                        Set(0, 0, 0);        // Black
                        break;

                    case ETerminalColor.Red:
                        Set(205, 0, 0);      // Red
                        break;

                    case ETerminalColor.Green:
                        Set(0, 205, 0);      // Green
                        break;

                    case ETerminalColor.Yellow:
                        Set(205, 205, 0);    // Yellow
                        break;

                    case ETerminalColor.Blue:
                        Set(0, 0, 205);      // Blue
                        break;

                    case ETerminalColor.Magenta:
                        Set(205, 0, 205);    // Magenta
                        break;

                    case ETerminalColor.Cyan:
                        Set(0, 205, 205);    // Cyan
                        break;

                    case ETerminalColor.White:
                        Set(205, 205, 205);  // White
                        break;
                }
            }
        }
    }
}
