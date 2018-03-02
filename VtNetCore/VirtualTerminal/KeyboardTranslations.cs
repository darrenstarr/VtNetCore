namespace VtNetCore.VirtualTerminal
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Provides static data for translating keystrokes from textual format to VT100... format
    /// </summary>
    internal static class KeyboardTranslations
    {
        /// <summary>
        /// Translates a key sequence from text to DEC VT byte codes
        /// </summary>
        /// <param name="key">The key formatted as text as seen in the dictionary below.</param>
        /// <param name="applicationMode">Set to true if the application mode mapping is desired</param>
        /// <returns></returns>
        public static byte[] GetKeySequence(string key, bool applicationMode)
        {
            if (KeyTranslations.TryGetValue(key, out KeyboardTranslation translation))
                return applicationMode ? translation.ApplicationMode : translation.NormalMode;

            return null;
        }

        /// <summary>
        /// The translation table itself.
        /// </summary>
        private static readonly Dictionary<string, KeyboardTranslation> KeyTranslations = new Dictionary<string, KeyboardTranslation>
        {
            { "Ctrl+Tab",       new KeyboardTranslation { NormalMode = RAW('\t'),   ApplicationMode = RAW('\t')   } },
            { "ESC",            new KeyboardTranslation { NormalMode = RAW('\x1B'), ApplicationMode = RAW('\x1B') } },

            { "F1",             new KeyboardTranslation { NormalMode = SS3("P"),    ApplicationMode = SS3("P")    } },
            { "F2",             new KeyboardTranslation { NormalMode = SS3("Q"),    ApplicationMode = SS3("Q")    } },
            { "F3",             new KeyboardTranslation { NormalMode = SS3("R"),    ApplicationMode = SS3("R")    } },
            { "F4",             new KeyboardTranslation { NormalMode = SS3("S"),    ApplicationMode = SS3("S")    } },
            { "F5",             new KeyboardTranslation { NormalMode = CSI("15~"),  ApplicationMode = CSI("15~")  } },
            { "F6",             new KeyboardTranslation { NormalMode = CSI("17~"),  ApplicationMode = CSI("17~")  } },
            { "F7",             new KeyboardTranslation { NormalMode = CSI("18~"),  ApplicationMode = CSI("18~")  } },
            { "F8",             new KeyboardTranslation { NormalMode = CSI("19~"),  ApplicationMode = CSI("19~")  } },
            { "F9",             new KeyboardTranslation { NormalMode = CSI("20~"),  ApplicationMode = CSI("20~")  } },
            { "F10",            new KeyboardTranslation { NormalMode = CSI("21~"),  ApplicationMode = CSI("21~")  } },
            { "F11",            new KeyboardTranslation { NormalMode = CSI("23~"),  ApplicationMode = CSI("23~")  } },
            { "F12",            new KeyboardTranslation { NormalMode = CSI("24~"),  ApplicationMode = CSI("24~")  } },

            { "Up",             new KeyboardTranslation { NormalMode = CSI("A"),    ApplicationMode = ESC("OA")    } },
            { "Down",           new KeyboardTranslation { NormalMode = CSI("B"),    ApplicationMode = ESC("OB")    } },
            { "Right",          new KeyboardTranslation { NormalMode = CSI("C"),    ApplicationMode = ESC("OC")    } },
            { "Left",           new KeyboardTranslation { NormalMode = CSI("D"),    ApplicationMode = ESC("OD")    } },

            { "Home",           new KeyboardTranslation { NormalMode = CSI("1~"),   ApplicationMode = CSI("1~")    } },
            { "Insert",         new KeyboardTranslation { NormalMode = CSI("2~"),   ApplicationMode = CSI("2~")    } },
            { "Delete",         new KeyboardTranslation { NormalMode = CSI("3~"),   ApplicationMode = CSI("3~")    } },
            { "End",            new KeyboardTranslation { NormalMode = CSI("4~"),   ApplicationMode = CSI("4~")    } },
            { "PageUp",         new KeyboardTranslation { NormalMode = CSI("5~"),   ApplicationMode = CSI("5~")    } },
            { "PageDown",       new KeyboardTranslation { NormalMode = CSI("6~"),   ApplicationMode = CSI("6~")    } },
        };

        /// <summary>
        /// Create a byte array prefixed with ESC (0x1B)
        /// </summary>
        /// <param name="command">The command text to append</param>
        /// <returns>A byte array as specified</returns>
        private static byte[] ESC(string command)
        {
            return (new byte[] { 0x1B }).Concat(Encoding.ASCII.GetBytes(command)).ToArray();
        }

        /// <summary>
        /// Create a byte array prefixed with ESC (0x1B) '['
        /// </summary>
        /// <param name="command">The command text to append</param>
        /// <returns>A byte array as specified</returns>
        private static byte[] CSI(string command)
        {
            return (new byte[] { 0x1B, (byte)'[' }).Concat(Encoding.ASCII.GetBytes(command)).ToArray();
        }

        /// <summary>
        /// Create a byte array prefixed with SS3 (0x8F)
        /// </summary>
        /// <param name="command">The command text to append</param>
        /// <returns>A byte array as specified</returns>
        private static byte[] SS3(string command)
        {
            return (new byte[] { 0x8F }).Concat(Encoding.ASCII.GetBytes(command)).ToArray();
        }

        /// <summary>
        /// Create a byte array without an escape prefix
        /// </summary>
        /// <param name="command">The command text to convert</param>
        /// <returns>A byte array as specified</returns>
        private static byte[] RAW(char ch)
        {
            return new byte[] { (byte)ch };
        }

    }
}
