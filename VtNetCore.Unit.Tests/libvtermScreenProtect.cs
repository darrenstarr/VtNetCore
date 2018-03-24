namespace VtNetCoreUnitTests
{
    using System.Text;
    using VtNetCore.VirtualTerminal;
    using VtNetCore.XTermParser;
    using Xunit;

    public class LibvtermScreenProtect
    {
        private void Push(DataConsumer d, string s)
        {
            d.Push(Encoding.UTF8.GetBytes(s));
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string ExpectBeforeErasure =
            "ABCdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab ";

        public static readonly string ExpectAfterErasure =
            " B                                                                               ";

        public static readonly string ExpectAfterNonSelectiveErasure =
            "                                                                                 ";

        // !Selective erase
        // RESET
        // PUSH "A\e[1\"qB\e[\"qC"
        //   ?screen_chars 0,0,1,3 = 0x41,0x42,0x43
        // PUSH "\e[G\e[?J"
        //   ?screen_chars 0,0,1,3 = 0x20,0x42
        [Fact]
        public void SelectiveErasure()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "A\e[1\"qB\e[\"qC"
            //   ?screen_chars 0,0,1,3 = 0x41,0x42,0x43
            Push(d, "A".DECSCA(1).T("B").DECSCA().T("C"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectBeforeErasure, s);

            // PUSH "\e[G\e[?J"
            //   ?screen_chars 0,0,1,3 = 0x20,0x42
            Push(d, "".CHA().DECSED());
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectAfterErasure, s);
        }

        // !Non-selective erase
        // RESET
        // PUSH "A\e[1\"qB\e[\"qC"
        //   ?screen_chars 0,0,1,3 = 0x41,0x42,0x43
        // PUSH "\e[G\e[J"
        //   ?screen_chars 0,0,1,3 = 
        [Fact]
        public void NonSelectiveErasure()
        {
            string s;
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);
            t.TestPatternScrollingDiagonalLower();

            // PUSH "A\e[1\"qB\e[\"qC"
            //   ?screen_chars 0,0,1,3 = 0x41,0x42,0x43
            Push(d, "A".DECSCA(1).T("B").DECSCA().T("C"));
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectBeforeErasure, s);

            // PUSH "\e[G\e[J"
            //   ?screen_chars 0,0,1,3 = 
            Push(d, "".CHA().EL());
            s = t.GetVisibleChars(0, 0, 81);
            Assert.Equal(ExpectAfterNonSelectiveErasure, s);
        }
    }
}
