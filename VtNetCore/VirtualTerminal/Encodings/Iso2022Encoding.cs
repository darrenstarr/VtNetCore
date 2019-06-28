using System;
using System.Collections.Generic;
using System.Text;
using VtNetCore.VirtualTerminal.Enums;

namespace VtNetCore.VirtualTerminal.Encodings
{
    public static class Iso2022Encoding
    {
        public static readonly Dictionary<char, char> C0 = new Dictionary<char, char>
        {
            { '_', ' ' },      // 5/9 - Blank
            { '`', '\u25C6' }, // 6/0 - U+25C6 # BLACK DIAMOND
            { 'a', '\u2592' }, // 6/1 - U+2592 # MEDIUM SHADE (checkerboard)
            { 'b', '\u2409' }, // 6/2 - U+2409 # SYMBOL FOR HORIZONTAL TAB
            { 'c', '\u240C' }, // 6/3 - U+240C # SYMBOL FOR FORM FEED
            { 'd', '\u240D' }, // 6/4 - U+240D # SYMBOL FOR CARRIAGE RETURN
            { 'e', '\u240A' }, // 6/5 - U+240A # SYMBOL FOR LINE FEED
            { 'f', '\u00B0' }, // 6/6 - U+00B0 # DEGREE SIGN
            { 'g', '\u00B1' }, // 6/7 - U+00B1 # PLUS-MINUS SIGN (plus or minus)
            { 'h', '\u2424' }, // 6/8 - U+2424 # SYMBOL FOR NEW LINE
            { 'i', '\u240B' }, // 6/9 - U+240B # SYMBOL FOR VERTICAL TAB
            { 'j', '\u2518' }, // 6/10 - U+2518 # BOX DRAWINGS LIGHT UP AND LEFT (bottom-right corner)
            { 'k', '\u2510' }, // 6/11 - U+2510 # BOX DRAWINGS LIGHT DOWN AND LEFT (top-right corner)
            { 'l', '\u250C' }, // 6/12 - U+250C # BOX DRAWINGS LIGHT DOWN AND RIGHT (top-left corner)
            { 'm', '\u2514' }, // 6/13 - U+2514 # BOX DRAWINGS LIGHT UP AND RIGHT (bottom-left corner)
            { 'n', '\u253C' }, // 6/14 - U+253C # BOX DRAWINGS LIGHT VERTICAL AND HORIZONTAL (crossing lines)
            { 'o', '\u23BA' }, // 6/15 - U+23BA # HORIZONTAL SCAN LINE-1
            { 'p', '\u23BB' }, // 7/0 - U+23BB # HORIZONTAL SCAN LINE-3
            { 'q', '\u2500' }, // 7/1 - U+2500 # BOX DRAWINGS LIGHT HORIZONTAL
            { 'r', '\u23BC' }, // 7/2 - U+23BC # HORIZONTAL SCAN LINE-7
            { 's', '\u23BD' }, // 7/3 - U+23BD # HORIZONTAL SCAN LINE-9
            { 't', '\u251C' }, // 7/4 - U+251C # BOX DRAWINGS LIGHT VERTICAL AND RIGHT
            { 'u', '\u2524' }, // 7/5 - U+2524 # BOX DRAWINGS LIGHT VERTICAL AND LEFT
            { 'v', '\u2534' }, // 7/6 - U+2534 # BOX DRAWINGS LIGHT UP AND HORIZONTAL
            { 'w', '\u252C' }, // 7/7 - U+252C # BOX DRAWINGS LIGHT DOWN AND HORIZONTAL
            { 'x', '\u2502' }, // 7/8 - U+2502 # BOX DRAWINGS LIGHT VERTICAL
            { 'y', '\u2A7D' }, // 7/9 - U+2A7D # LESS-THAN OR SLANTED EQUAL-TO
            { 'z', '\u2A7E' }, // 7/10 - U+2A7E # GREATER-THAN OR SLANTED EQUAL-TO
            { '{', '\u03C0' }, // 7/11 - U+03C0 # GREEK SMALL LETTER PI
            { '|', '\u2260' }, // 7/12 - U+2260 # NOT EQUAL TO
            { '}', '\u00A3' }, // 7/13 - U+00A3 # POUND SIGN
            { '~', '\u00B7' }, // 7/14 - U+00B7 # MIDDLE DOT
        };

        public static readonly Dictionary<char, char> Latin1 = new Dictionary<char, char>
        {
            { '!', '¡' },
            { '"', '¢' },
            { '#', '£' },
            { '$', '¤' },
            { '%', '¥' },
            { '&', '¦' },
            { '\'', '§' },
            { '(', '¨' },
            { ')', '©' },
            { '*', 'ª' },
            { '+', '«' },
            { ',', '¬' },
            //{ '-', '\u00AD' },
            { '.', '®' },
            { '/', '¯' },
            { '0', '°' },
            { '1', '±' },
            { '2', '²' },
            { '3', '³' },
            { '4', '´' },
            { '5', 'µ' },
            { '6', '¶' },
            { '7', '·' },
            { '8', '¸' },
            { '9', '¹' },
            { ':', 'º' },
            { ';', '»' },
            { '<', '¼' },
            { '=', '½' },
            { '>', '¾' },
            { '?', '¿' },
            { '@', 'À' },
            { 'A', 'Á' },
            { 'B', 'Â' },
            { 'C', 'Ã' },
            { 'D', 'Ä' },
            { 'E', 'Å' },
            { 'F', 'Æ' },
            { 'G', 'Ç' },
            { 'H', 'È' },
            { 'I', 'É' },
            { 'J', 'Ê' },
            { 'K', 'Ë' },
            { 'L', 'Ì' },
            { 'M', 'Í' },
            { 'N', 'Î' },
            { 'O', 'Ï' },
            { 'P', 'Ð' },
            { 'Q', 'Ñ' },
            { 'R', 'Ò' },
            { 'S', 'Ó' },
            { 'T', 'Ô' },
            { 'U', 'Õ' },
            { 'V', 'Ö' },
            { 'W', '×' },
            { 'X', 'Ø' },
            { 'Y', 'Ù' },
            { 'Z', 'Ú' },
            { '[', 'Û' },
            { '\\', 'Ü' },
            { ']', 'Ý' },
            { '^', 'Þ' },
            { '_', 'ß' },
            { '`', 'à' },
            { 'a', 'á' },
            { 'b', 'â' },
            { 'c', 'ã' },
            { 'd', 'ä' },
            { 'e', 'å' },
            { 'f', 'æ' },
            { 'g', 'ç' },
            { 'h', 'è' },
            { 'i', 'é' },
            { 'j', 'ê' },
            { 'k', 'ë' },
            { 'l', 'ì' },
            { 'm', 'í' },
            { 'n', 'î' },
            { 'o', 'ï' },
            { 'p', 'ð' },
            { 'q', 'ñ' },
            { 'r', 'ò' },
            { 's', 'ó' },
            { 't', 'ô' },
            { 'u', 'õ' },
            { 'v', 'ö' },
            { 'w', '÷' },
            { 'x', 'ø' },
            { 'y', 'ù' },
            { 'z', 'ú' },
            { '{', 'û' },
            { '|', 'ü' },
            { '}', 'ý' },
            { '~', 'þ' },
            { (char)127, 'ÿ' }
        };

        public static readonly Dictionary<char, char> Finnish = new Dictionary<char, char>
        {
            { '\\', 'Ö' },
            { ']', 'Å' },
            { '^', 'Ü' },
            { '`', 'é' },
            { '{', 'ä' },
            { '|', 'ö' },
            { '}', 'å' },
            { '~', 'ü' },
        };

        public static readonly Dictionary<char, char> French = new Dictionary<char, char>
        {
            { '#', '£' },
            { '[', '°' },
            { '\\', 'ç' },
            { ']', '§' },
            { '{', 'é' },
            { '|', 'ù' },
            { '}', 'è' },
            { '~', '¨' },
        };

        public static readonly Dictionary<char, char> FrenchCanadian = new Dictionary<char, char>
        {
            { '@', 'à' },
            { '[', 'â' },
            { '\\', 'ç' },
            { ']', 'ê' },
            { '{', 'é' },
            { '^', 'î' },
            { '`', 'ô' },
            { '|', 'ù' },
            { '}', 'è' },
            { '~', 'û' },
        };

        public static readonly Dictionary<char, char> German = new Dictionary<char, char>
        {
            { '@', '§' },
            { '[', 'Ä' },
            { '\\', 'Ö' },
            { ']', 'Ü' },
            { '{', 'ä' },
            { '|', 'ö' },
            { '}', 'ü' },
            { '~', 'β' },
        };

        public static readonly Dictionary<char, char> Italian = new Dictionary<char, char>
        {
            { '#', '£' },
            { '@', '§' },
            { '[', '°' },
            { '\\', 'ç' },
            { ']', 'é' },
            { '`', 'ù' },
            { '{', 'à' },
            { '|', 'ò' },
            { '}', 'è' },
            { '~', 'ì' },
        };

        public static readonly Dictionary<char, char> NorwegianDanish = new Dictionary<char, char>
        {
            { '[', 'Æ' },
            { '\\', 'Ø' },
            { ']', 'Å' },
            { '{', 'æ' },
            { '|', 'ø' },
            { '}', 'å' },
        };

        public static readonly Dictionary<char, char> Portuguese = new Dictionary<char, char>
        {
            { '[', 'Ã' },
            { '\\', 'Ç' },
            { ']', 'Õ' },
            { '{', 'ã' },
            { '|', 'ç' },
            { '}', 'õ' },
        };

        public static readonly Dictionary<char, char> Spanish = new Dictionary<char, char>
        {
            { '#', '£' },
            { '@', '§' },
            { '[', '¡' },
            { '\\', 'Ñ' },
            { ']', '¿' },
            { '{', '°' },
            { '|', 'ñ' },
            { '}', 'ç' },
        };

        public static readonly Dictionary<char, char> DecTechnical = new Dictionary<char, char>
        {
            { (char)0x21, '\u23B7' },
            { (char)0x22, '\u250C' },
            { (char)0x23, '\u2500' },
            { (char)0x24, '\u2320' },
            { (char)0x25, '\u2321' },
            { (char)0x26, '\u2502' },
            { (char)0x27, '\u23A1' },
            { (char)0x28, '\u23A3' },
            { (char)0x29, '\u23A4' },
            { (char)0x2A, '\u23A6' },
            { (char)0x2B, '\u239B' },
            { (char)0x2C, '\u239D' },
            { (char)0x2D, '\u239E' },
            { (char)0x2E, '\u23A0' },
            { (char)0x2F, '\u23A8' },
            { (char)0x30, '\u23AC' },
            { (char)0x3C, '\u2264' },
            { (char)0x3D, '\u2260' },
            { (char)0x3E, '\u2265' },
            { (char)0x3F, '\u222B' },
            { (char)0x40, '\u2234' },
            { (char)0x41, '\u221D' },
            { (char)0x42, '\u221E' },
            { (char)0x43, '\u00F7' },
            { (char)0x44, '\u0394' },
            { (char)0x45, '\u2207' },
            { (char)0x46, '\u03A6' },
            { (char)0x47, '\u0393' },
            { (char)0x48, '\u223C' },
            { (char)0x49, '\u2243' },
            { (char)0x4A, '\u0398' },
            { (char)0x4B, '\u00D7' },
            { (char)0x4C, '\u039B' },
            { (char)0x4D, '\u21D4' },
            { (char)0x4E, '\u21D2' },
            { (char)0x4F, '\u2261' },
            { (char)0x50, '\u03A0' },
            { (char)0x51, '\u03A8' },
            { (char)0x53, '\u03A3' },
            { (char)0x56, '\u221A' },
            { (char)0x57, '\u03A9' },
            { (char)0x58, '\u039E' },
            { (char)0x59, '\u03A5' },
            { (char)0x5A, '\u2282' },
            { (char)0x5B, '\u2283' },
            { (char)0x5C, '\u2229' },
            { (char)0x5D, '\u222A' },
            { (char)0x5E, '\u2227' },
            { (char)0x5F, '\u2228' },
            { (char)0x60, '\u00AC' },
            { (char)0x61, '\u03B1' },
            { (char)0x62, '\u03B2' },
            { (char)0x63, '\u03C7' },
            { (char)0x64, '\u03B4' },
            { (char)0x65, '\u03B5' },
            { (char)0x66, '\u03C6' },
            { (char)0x67, '\u03B3' },
            { (char)0x68, '\u03B7' },
            { (char)0x69, '\u03B9' },
            { (char)0x6A, '\u03B8' },
            { (char)0x6B, '\u03BA' },
            { (char)0x6C, '\u03BB' },
            { (char)0x6E, '\u03BD' },
            { (char)0x6F, '\u2202' },
            { (char)0x70, '\u03C0' },
            { (char)0x71, '\u03C8' },
            { (char)0x72, '\u03C1' },
            { (char)0x73, '\u03C3' },
            { (char)0x74, '\u03C4' },
            { (char)0x76, '\u0192' },
            { (char)0x77, '\u03C9' },
            { (char)0x78, '\u03BE' },
            { (char)0x79, '\u03C5' },
            { (char)0x7A, '\u03B6' },
            { (char)0x7B, '\u2190' },
            { (char)0x7C, '\u2191' },
            { (char)0x7D, '\u2192' },
            { (char)0x7E, '\u2193' },
        };

        public static char DecodeChar(char inChar, ECharacterSet characterSet, bool nationalReplacementCharacterSet)
        {
            char replacement;

            switch(characterSet)
            {
                case ECharacterSet.Latin1:
                case ECharacterSet.DecSupplemental:
                case ECharacterSet.DecSupplementalGraphic:
                    if (nationalReplacementCharacterSet)
                    {
                        if (inChar == '#')
                            return '\u00a3';
                    }
                    else
                    {
                        if (Latin1.TryGetValue(inChar, out replacement))
                            return replacement;
                    }
                    return inChar;

                case ECharacterSet.C0:
                    if (C0.TryGetValue(inChar, out replacement))
                        return replacement;
                    return inChar;

                case ECharacterSet.DecTechnical:
                    if (DecTechnical.TryGetValue(inChar, out replacement))
                        return replacement;
                    return '\uFFFD';

                case ECharacterSet.Finnish:
                    if (Finnish.TryGetValue(inChar, out replacement))
                        return replacement;
                    return inChar;

                case ECharacterSet.French:
                    if (French.TryGetValue(inChar, out replacement))
                        return replacement;
                    return inChar;

                case ECharacterSet.FrenchCanadian:
                    if (FrenchCanadian.TryGetValue(inChar, out replacement))
                        return replacement;
                    return inChar;

                case ECharacterSet.German:
                    if (German.TryGetValue(inChar, out replacement))
                        return replacement;
                    return inChar;

                case ECharacterSet.Italian:
                    if (Italian.TryGetValue(inChar, out replacement))
                        return replacement;
                    return inChar;

                case ECharacterSet.Portuguese:
                    if (Portuguese.TryGetValue(inChar, out replacement))
                        return replacement;
                    return inChar;

                case ECharacterSet.NorwegianDanish:
                    if (NorwegianDanish.TryGetValue(inChar, out replacement))
                        return replacement;
                    return inChar;

                case ECharacterSet.Spanish:
                    if (Spanish.TryGetValue(inChar, out replacement))
                        return replacement;
                    return inChar;

                default:
                    if (inChar == (char)127)
                        return ' ';

                    return inChar;
            }
        }
    }
}
