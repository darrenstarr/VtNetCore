using System;
using System.Collections.Generic;
using System.Text;
using VtNetCore.VirtualTerminal.Enums;

namespace VtNetCore.VirtualTerminal.Encodings
{
    public static class Iso2022Encoding
    {
        public static readonly Dictionary<Char, Char> C0 = new Dictionary<char, char>
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

        public static readonly Dictionary<Char, Char> Latin1 = new Dictionary<char, char>
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

        public static char DecodeChar(char inChar, ECharacterSet characterSet, bool nationalReplacementCharacterSet)
        {
            switch(characterSet)
            {
                case ECharacterSet.Latin1:
                    if (nationalReplacementCharacterSet)
                    {
                        if (inChar == '#')
                            return '\u00a3';
                    }
                    else
                    {
                        if (Latin1.TryGetValue(inChar, out char latin1Value))
                            return latin1Value;
                    }
                    return inChar;

                case ECharacterSet.C0:
                    if (C0.TryGetValue(inChar, out char c0Value))
                        return c0Value;
                    return inChar;

                default:
                    if (inChar == (char)127)
                        return ' ';

                    return inChar;
            }
        }
    }
}
