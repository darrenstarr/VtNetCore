using System;
using System.Collections.Generic;
using System.Text;
using VtNetCore.VirtualTerminal;
using VtNetCore.XTermParser;
using Xunit;

namespace VtNetCoreUnitTests
{
    public class NanoBug
    {
        private void Push(DataConsumer d, string s)
        {
            d.Push(Encoding.UTF8.GetBytes(s));
        }

        private bool IsCursor(VirtualTerminalController t, int row, int column)
        {
            return t.ViewPort.CursorPosition.Row == row && t.ViewPort.CursorPosition.Column == column;
        }

        //  "000000000111111111122222222223333333333444444444455555555556666666666777777777788
        //  "123456789012345678901234567890123456789012345678901234567890123456789012345678901"
        public static readonly string NanoBeforeScroll =
            "  GNU nano 2.5.3         File: pipes.sh                         " + "\n" +    // 1
            "                                                                " + "\n" +    // 2
            "#                                                               " + "\n" +    // 3
            "# The above copyright notice and this permission notice shall b$" + "\n" +    // 4
            "# all copies or substantial portions of the Software.           " + "\n" +    // 5
            "#                                                               " + "\n" +    // 6
            "# THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIN$" + "\n" +  // 7
            "# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCH$" + "\n" +    // 8
            "# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO E$" + "\n" +    // 9
            "# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES$" + "\n" +    // 10
            "# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWIS$" + "\n" +    // 11
            "# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER$" + "\n" +    // 12
            "# SOFTWARE.                                                     " + "\n" +    // 13
            "                                                                " + "\n" +    // 14
            "                                                                " + "\n" +    // 15
            "                                                                " + "\n" +    // 16
            "^G Get Help ^O Write Out^W Where Is ^K Cut Text ^J Justify      " + "\n" +    // 17
            "^X Exit     ^R Read File^\\ Replace  ^U Uncut Tex^T To Linter    ";           // 18

        public static readonly string Phase0 =
            "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijkl" + "\n" +    // 1
            "babcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijk" + "\n" +    // 2
            "cbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghij" + "\n" +    // 3
            "dcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghi" + "\n" +    // 4
            "edcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefgh" + "\n" +    // 5
            "fedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefg" + "\n" +    // 6
            "gfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdef" + "\n" +    // 7
            "hgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcde" + "\n" +    // 8
            "ihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcd" + "\n" +    // 9
            "jihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabc" + "\n" +    // 10
            "kjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab" + "\n" +    // 11
            "lkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyza" + "\n" +    // 12
            "mlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" + "\n" +    // 13
            "nmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxy" + "\n" +    // 14
            "onmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwx" + "\n" +    // 15
            "ponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvw" + "\n" +    // 16
            "qponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuv" + "\n" +    // 17
            "rqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstu";            // 18

        public static readonly string Phase1 =
            "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijkl" + "\n" +
            "babcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijk" + "\n" +
            "jihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabc" + "\n" +
            "kjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzab" + "\n" +
            "lkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyza" + "\n" +
            "mlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" + "\n" +
            "nmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxy" + "\n" +
            "onmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwx" + "\n" +
            "ponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvw" + "\n" +
            "                                                                " + "\n" +
            "                                                                " + "\n" +
            "                                                                " + "\n" +
            "                                                                " + "\n" +
            "                                                                " + "\n" +
            "                                                                " + "\n" +
            "                                                                " + "\n" +
            "qponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuv" + "\n" +
            "rqponmlkjihgfedcbabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstu";

        /// <summary>
        /// Tests that top and bottom margins are held even after cursor restore.
        /// </summary>
        /// <remarks>
        /// It seems like it would make sense to try and implement libvterm's save cursor tests
        /// </remarks>
        [Fact]
        public void NanoScrollingBug()
        {
            var t = new VirtualTerminalController();
            var d = new DataConsumer(t);
            t.ResizeView(64, 18);
            t.TestPatternScrollingDiagonalLower();

            // Move to cursor position 14,0 before starting
            Push(d, "".CUP(15, 1));
            Assert.True(IsCursor(t, 14, 0));

            // ␛7
            Push(d, "".DECSC());
            // ␛[3;16r
            Push(d, "".STBM(3, 16));

            // ␛8
            Push(d, "".DECRC());
            Assert.True(IsCursor(t, 14, 0));

            // ␛[16d
            Push(d, "".VPA(16));
            Assert.True(IsCursor(t, 15, 0));

            // ␛[7S
            Push(d, "".SU(7));
        }
    }
}
