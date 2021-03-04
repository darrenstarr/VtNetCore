namespace VtNetCore.XTermParser
{
    using System;
    using System.Collections.Generic;
    using VtNetCore.Exceptions;
    using VtNetCore.VirtualTerminal.Enums;
    using VtNetCore.XTermParser.SequenceType;

    public class XTermSequenceReader
    {
        private static TerminalSequence ConsumeCSI(XTermInputBuffer stream)
        {
            stream.PushState();

            bool atStart = true;
            bool isQuery = false;
            bool isSend = false;
            bool isBang = false;
            bool isEquals = false;
            char? modifier = null;

            int currentParameter = -1;
            List<int> Parameters = new List<int>();
            List<TerminalSequence> ProcesFirst = new List<TerminalSequence>();

            while (true)
            {
                var next = stream.Read();

                if (atStart && next == '?')
                    isQuery = true;
                else if (atStart && next == '>')
                    isSend = true;
                else if (atStart && next == '!')
                    isBang = true;
                else if (atStart && next == '=')
                    isEquals = true;
                else if (next == ';')
                {
                    if (currentParameter == -1)
                    {
                        //currentParameter = 1;       // ctrlseqs.txt seems to always default to 1 here. Might not be a great idea
                        atStart = false;
                        //throw new EscapeSequenceException("Invalid position for ';' in CSI", stream.Stacked);
                    }

                    Parameters.Add(currentParameter);
                    currentParameter = -1;
                }
                else if (char.IsDigit(next))
                {
                    atStart = false;
                    if (currentParameter == -1)
                        currentParameter = Convert.ToInt32(next - '0');
                    else
                        currentParameter = (currentParameter * 10) + Convert.ToInt32(next - '0');
                }
                else if (next == '$' || next == '"' || next == ' ' || next == '\'')
                {
                    if (modifier.HasValue)
                        throw new EscapeSequenceException("There appears to be two modifiers in a row", stream.Stacked);

                    if (currentParameter != -1)
                    {
                        Parameters.Add(currentParameter);
                        currentParameter = -1;
                    }

                    modifier = next;
                }
                else if (next == '\b' || next == '\r' || next == '\u000B')
                {
                    // Trash chars that have to be processed before this sequence
                    ProcesFirst.Add(
                        new CharacterSequence
                        {
                            Character = next
                        }
                    );
                }
                else if (next == '\0')
                {
                    // Trash null characters. Telnet is injecting them after carriage returns for
                    // some horrible reason
                }
                else
                {
                    if (currentParameter != -1)
                    {
                        Parameters.Add(currentParameter);
                        currentParameter = -1;
                    }

                    var csi = new CsiSequence
                    {
                        Parameters = Parameters,
                        IsQuery = isQuery,
                        IsSend = isSend,
                        IsBang = isBang,
                        IsEquals = isEquals,
                        Command = (modifier.HasValue ? modifier.Value.ToString() : "") + next.ToString(),
                        ProcessFirst = ProcesFirst.Count > 0 ? ProcesFirst : null
                    };

                    stream.Commit();

                    //System.Diagnostics.Debug.WriteLine(csi.ToString());

                    return csi;
                }
            }
        }

        private static TerminalSequence ConsumeOSC(XTermInputBuffer stream)
        {
            stream.PushState();

            string command = "";
            bool readingCommand = false;
            bool atStart = true;
            bool isQuery = false;
            bool isSend = false;
            bool isBang = false;
            char? modifier = null;

            int currentParameter = -1;
            List<int> Parameters = new List<int>();

            while (true)
            {
                var next = stream.Read();

                if (readingCommand)
                {
                    if (next == 0x07 || next == 0x9C)        // BEL or ST
                    {
                        var osc = new OscSequence
                        {
                            Parameters = Parameters,
                            IsQuery = isQuery,
                            IsSend = isSend,
                            IsBang = isBang,
                            Command = command
                        };

                        stream.Commit();

                        //System.Diagnostics.Debug.WriteLine(osc.ToString());

                        return osc;
                    }
                    else
                    {
                        command += next;
                    }
                }
                else
                {
                    if (atStart && next == '?')
                        isQuery = true;
                    else if (atStart && next == '>')
                        isSend = true;
                    else if (atStart && next == '!')
                        isBang = true;
                    else if (next == ';')
                    {
                        if (currentParameter == -1)
                            throw new EscapeSequenceException("Invalid position for ';' in OSC", stream.Stacked);

                        Parameters.Add(currentParameter);
                        currentParameter = -1;
                        readingCommand = true;
                    }
                    else if (char.IsDigit(next))
                    {
                        atStart = false;
                        if (currentParameter == -1)
                            currentParameter = Convert.ToInt32(next - '0');
                        else
                            currentParameter = (currentParameter * 10) + Convert.ToInt32(next - '0');
                    }
                    else if (next == '$' || next == '"' || next == ' ')
                    {
                        if (modifier.HasValue)
                            throw new EscapeSequenceException("There appears to be two modifiers in a row", stream.Stacked);

                        if (currentParameter != -1)
                        {
                            Parameters.Add(currentParameter);
                            currentParameter = -1;
                        }

                        modifier = next;
                    }
                    else
                    {
                        if (currentParameter != -1)
                        {
                            Parameters.Add(currentParameter);
                            currentParameter = -1;
                        }

                        command += next;
                        readingCommand = true;
                    }
                }
            }
        }

        private static TerminalSequence ConsumeCompliance(XTermInputBuffer stream)
        {
            var next = stream.Read();

            var compliance = new OscSequence
            {
                Command = next.ToString()
            };

            stream.Commit();

            //System.Diagnostics.Debug.WriteLine(compliance.ToString());

            return compliance;
        }

        private static TerminalSequence ConsumeCharacterSize(XTermInputBuffer stream)
        {
            var next = stream.Read();

            ECharacterSize size;
            switch(next)
            {
                case '3':
                    size = ECharacterSize.DoubleHeightLineTop;
                    break;
                case '4':
                    size = ECharacterSize.DoubleHeightLineBottom;
                    break;
                default:
                case '5':
                    size = ECharacterSize.SingleWidthLine;
                    break;
                case '6':
                    size = ECharacterSize.DoubleWidthLine;
                    break;
                case '8':
                    size = ECharacterSize.ScreenAlignmentTest;
                    break;
            }

            var characterSize = new CharacterSizeSequence
            {
                Size = size
            };

            stream.Commit();

            //System.Diagnostics.Debug.WriteLine(characterSize.ToString());

            return characterSize;
        }

        private static TerminalSequence ConsumeUnicode(XTermInputBuffer stream)
        {
            var next = stream.Read();

            var unicode = new UnicodeSequence
            {
                Command = next.ToString()
            };

            stream.Commit();

            //System.Diagnostics.Debug.WriteLine(unicode.ToString());

            return unicode;
        }

        private static TerminalSequence ConsumeCharacterSet(char set, XTermInputBuffer stream)
        {
            var next = stream.Read();

            ECharacterSetMode mode;
            switch (set)
            {
                case '(':
                default:
                    mode = ECharacterSetMode.IsoG0;
                    break;

                case ')':
                    mode = ECharacterSetMode.IsoG1;
                    break;

                case '*':
                    mode = ECharacterSetMode.IsoG2;
                    break;

                case '+':
                    mode = ECharacterSetMode.IsoG3;
                    break;

                case '-':
                    mode = ECharacterSetMode.Vt300G1;
                    break;

                case '.':
                    mode = ECharacterSetMode.Vt300G2;
                    break;

                case '/':
                    mode = ECharacterSetMode.Vt300G3;
                    break;
            }

            ECharacterSet characterSet;
            switch (next)
            {
                case '0':
                    characterSet = ECharacterSet.C0;
                    break;
                case '1':
                    characterSet = ECharacterSet.C1;
                    break;
                case '2':
                    characterSet = ECharacterSet.C2;
                    break;
                case 'A':
                    characterSet = ECharacterSet.Latin1;
                    break;
                case '4':
                    characterSet = ECharacterSet.Dutch;
                    break;
                case 'C':
                case '5':
                    characterSet = ECharacterSet.Finnish;
                    break;
                case 'R':
                    characterSet = ECharacterSet.French;
                    break;
                case 'Q':
                    characterSet = ECharacterSet.FrenchCanadian;
                    break;
                case 'K':
                    characterSet = ECharacterSet.German;
                    break;
                case 'Y':
                    characterSet = ECharacterSet.Italian;
                    break;
                case 'E':
                case '6':
                case '`':
                    characterSet = ECharacterSet.NorwegianDanish;
                    break;
                case 'Z':
                    characterSet = ECharacterSet.Spanish;
                    break;
                case 'H':
                case '7':
                    characterSet = ECharacterSet.Swedish;
                    break;
                case '=':
                    characterSet = ECharacterSet.Swiss;
                    break;

                case '>':
                    characterSet = ECharacterSet.DecTechnical;
                    break;

                case '<':
                    characterSet = ECharacterSet.DecSupplemental;
                    break;

                case '%':
                    var num = stream.Read();
                    switch (num)
                    {
                        case '5':
                            characterSet = ECharacterSet.DecSupplementalGraphic;
                            break;
                        case '6':
                            characterSet = ECharacterSet.Portuguese;
                            break;
                        default:
                            characterSet = ECharacterSet.USASCII;
                            break;
                    }
                    break;

                default:
                    characterSet = ECharacterSet.USASCII;
                    break;

                case 'B':
                    characterSet = ECharacterSet.USASCII;
                    break;
            }

            stream.Commit();

            var characterSetSequence = new CharacterSetSequence
            {
                CharacterSet = characterSet,
                Mode = mode
            };

            //System.Diagnostics.Debug.WriteLine(characterSetSequence.ToString());

            return characterSetSequence;
        }

        private static TerminalSequence ConsumeEscapeSequence(XTermInputBuffer stream)
        {
            stream.PushState();
            var next = stream.Read();

            switch (next)
            {
                case '[':
                    return ConsumeCSI(stream);

                case ']':
                    return ConsumeOSC(stream);

                case 'P':
                    return ConsumeDeviceControlStringSequence(stream);

                case '#':
                    return ConsumeCharacterSize(stream);

                case ' ':
                    return ConsumeCompliance(stream);

                case '%':
                    return ConsumeUnicode(stream);

                case '(':
                case ')':
                case '*':
                case '+':
                case '-':
                case '.':
                case '/':
                    return ConsumeCharacterSet(next, stream);

                case 'Y':
                    var vt52mc = new Vt52MoveCursorSequence
                    {
                        Row = stream.ReadRaw() - ' ',
                        Column = stream.ReadRaw() - ' '
                    };

                    stream.Commit();

                    System.Diagnostics.Debug.WriteLine(vt52mc.ToString());
                    return vt52mc;

                default:
                    var esc = new EscapeSequence
                    {
                        Command = next.ToString()
                    };

                    stream.Commit();

                    //System.Diagnostics.Debug.WriteLine(esc.ToString());
                    return esc;
            }
        }

        private static TerminalSequence ConsumeSS2Sequence(XTermInputBuffer stream)
        {
            var next = stream.ReadRaw();

            var ss2 = new SS2Sequence
            {
                Command = next.ToString()
            };

            stream.Commit();

            //System.Diagnostics.Debug.WriteLine(ss2.ToString());
            return ss2;
        }

        private static TerminalSequence ConsumeSS3Sequence(XTermInputBuffer stream)
        {
            var next = stream.ReadRaw();

            var ss3 = new SS3Sequence
            {
                Command = next.ToString()
            };

            stream.Commit();

            //System.Diagnostics.Debug.WriteLine(ss3.ToString());
            return ss3;
        }

        private static TerminalSequence ConsumeDeviceControlStringSequence(XTermInputBuffer stream)
        {
            stream.PushState();

            string command = "";
            bool readingCommand = false;
            bool atStart = true;
            bool isQuery = false;
            bool isSend = false;
            bool isBang = false;
            char? modifier = null;

            int currentParameter = -1;
            List<int> Parameters = new List<int>();

            while (!stream.AtEnd)
            {
                var next = stream.Read();

                if (readingCommand)
                {
                    if (next == 0x07 || next == 0x9C)        // BEL or ST
                    {
                        var dcs = new DcsSequence
                        {
                            Parameters = Parameters,
                            IsQuery = isQuery,
                            IsSend = isSend,
                            IsBang = isBang,
                            Command = (modifier.HasValue ? modifier.Value.ToString() : "") + command
                        };

                        stream.Commit();

                        //System.Diagnostics.Debug.WriteLine(dcs.ToString());

                        return dcs;
                    }
                    else if(next == 0x1B)               // ESC
                    {
                        var stChar = stream.Read();
                        if(stChar == '\\')
                        {
                            var dcs = new DcsSequence
                            {
                                Parameters = Parameters,
                                IsQuery = isQuery,
                                IsSend = isSend,
                                IsBang = isBang,
                                Command = (modifier.HasValue ? modifier.Value.ToString() : "") + command
                            };

                            stream.Commit();

                            //System.Diagnostics.Debug.WriteLine(dcs.ToString());

                            return dcs;
                        }
                        else
                            throw new EscapeSequenceException("ESC \\ is needed to terminate DCS. Encounterd wrong character.", stream.Stacked);
                    }
                    else
                    {
                        command += next;
                    }
                }
                else
                {
                    if (atStart && next == '?')
                        isQuery = true;
                    else if (atStart && next == '>')
                        isSend = true;
                    else if (atStart && next == '!')
                        isBang = true;
                    else if (next == ';')
                    {
                        if (currentParameter == -1)
                            throw new EscapeSequenceException("Invalid position for ';' in DCS", stream.Stacked);

                        Parameters.Add(currentParameter);
                        currentParameter = -1;
                    }
                    else if (char.IsDigit(next))
                    {
                        atStart = false;
                        if (currentParameter == -1)
                            currentParameter = Convert.ToInt32(next - '0');
                        else
                            currentParameter = (currentParameter * 10) + Convert.ToInt32(next - '0');
                    }
                    else if (next == '$' || next == '"' || next == ' ')
                    {
                        if (modifier.HasValue)
                            throw new EscapeSequenceException("There appears to be two modifiers in a row", stream.Stacked);

                        if (currentParameter != -1)
                        {
                            Parameters.Add(currentParameter);
                            currentParameter = -1;
                        }

                        modifier = next;
                    }
                    else
                    {
                        if (currentParameter != -1)
                        {
                            Parameters.Add(currentParameter);
                            currentParameter = -1;
                        }

                        command += next;
                        readingCommand = true;
                    }
                }
            }

            stream.PopState();
            return null;
        }

        public static TerminalSequence ConsumeNextSequence(XTermInputBuffer stream, bool utf8)
        {
            stream.PushState();
            var next = stream.Read(utf8);

            TerminalSequence sequence = null;
            switch (next)
            {
                case '\u001b':      // ESC
                    sequence = ConsumeEscapeSequence(stream);
                    break;

                case '\u008e':      // SS2
                    sequence = ConsumeSS2Sequence(stream);
                    break;

                case '\u008f':      // SS3
                    sequence = ConsumeSS3Sequence(stream);
                    break;

                case '\u0090':      // DCS
                    sequence = ConsumeDeviceControlStringSequence(stream);
                    break;

                default:
                    break;
            }

            if (sequence == null)
            {
                sequence = new CharacterSequence
                {
                    Character = next
                };
                stream.Commit();
            }

            return sequence;
        }
    }
}
