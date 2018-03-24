using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using VtNetCore.VirtualTerminal;
using VtNetCore.XTermParser;
using Xunit;

namespace VtNetCoreUnitTests
{
    public class vttestVt52
    {
        private void Push(DataConsumer d, string s)
        {
            d.Push(Encoding.UTF8.GetBytes(s));
        }

        private bool IsCursor(VirtualTerminalController t, int row, int column)
        {
            return t.ViewPort.CursorPosition.Row == row && t.ViewPort.CursorPosition.Column == column;
        }

        public int MaxLines = 24;

        public static readonly string ExpectTestVt52Fill =
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar FooBar Bletch    " + "\n" +
            "                                                                                ";

        public static readonly string ExpectTestVt52ClearCup =
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                              nothing more.                     " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                ";

        public static readonly string ExpectTestVt52Backscroll =
            "                                                                                " + "\n" +
            "Back scroll (this should go away)                                               " + "\n" +
            "Back scroll (this should go away)                                               " + "\n" +
            "Back scroll (this should go away)                                               " + "\n" +
            "Back scroll (this should go away)                                               " + "\n" +
            "Back scroll (this should go away)                                               " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                              nothing more.THIS SHOULD GO AWAY! " + "\n" +
            "THIS SHOULD GO AWAY! THIS SHOULD GO AWAY! THIS SHOULD GO AWAY! THIS SHOULD GO AW" + "\n" +
            "AY! THIS SHOULD GO AWAY! THIS SHOULD GO AWAY! THIS SHOULD GO AWAY! THIS SHOULD G" + "\n" +
            "O AWAY! THIS SHOULD GO AWAY!                                                    " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                ";

        public static readonly string ExpectTestVt52EraseToEnd =
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                              nothing more.                     " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                " + "\n" +
            "                                                                                ";

        public static readonly string ExpectTestVt52Final =
            "         *************************************************************          " + "\n" +
            "         *!                                                         !*          " + "\n" +
            "         *!                                                         !*          " + "\n" +
            "         *!                                                         !*          " + "\n" +
            "         *!                                                         !*          " + "\n" +
            "         *!                                                         !*          " + "\n" +
            "         *!                                                         !*          " + "\n" +
            "         *!                                                         !*          " + "\n" +
            "         *!                                                         !*          " + "\n" +
            "         *!    The screen should be cleared, and have a centered    !*          " + "\n" +
            "         *!    rectangle of \"*\"s with \"!\"s on the inside to the     !*          " + "\n" +
            "         *!    left and right. Only this, and nothing more.         !*          " + "\n" +
            "         *!                                                         !*          " + "\n" +
            "         *!                                                         !*          " + "\n" +
            "         *!                                                         !*          " + "\n" +
            "         *!                                                         !*          " + "\n" +
            "         *!                                                         !*          " + "\n" +
            "         *!                                                         !*          " + "\n" +
            "         *!                                                         !*          " + "\n" +
            "         *!                                                         !*          " + "\n" +
            "         *!                                                         !*          " + "\n" +
            "         *!                                                         !*          " + "\n" +
            "         *!                                                         !*          " + "\n" +
            "         *************************************************************          " + "\n" +
            "                                                                                ";

        [Fact]
        public void TestVt52()
        {
            string s;
            var t = new VirtualTerminalController();
            t.Debugging = true;
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);

            // set_level(0);        /* Reset ANSI (VT100) mode, Set VT52 mode  */
            Push(d, "".SetVt52Mode());

            // t52home();   /* Cursor home     */
            // vt52ed();     /* Erase to end of screen  */
            // vt52home();   /* Cursor home     */
            Push(d, "".Vt52Home().Vt52ed().Vt52Home());

            // for (i = 0; i <= max_lines - 1; i++)
            // {
            //     for (j = 0; j <= 9; j++)
            //         printf("%s", "FooBar ");
            //     println("Bletch");
            // }
            for (var i = 0; i <= MaxLines - 1; i++)
            {
                for (var j = 0; j <= 9; j++)
                    Push(d, "FooBar ");

                Push(d, "Bletch\r\n");
            }

            s = t.GetScreenText();
            Assert.Equal(ExpectTestVt52Fill, s);

            // vt52home();   /* Cursor home     */
            // vt52ed();     /* Erase to end of screen  */
            Push(d, "".Vt52Home().Vt52ed());

            // vt52cup(7, 47);
            // printf("nothing more.");
            Push(d, "".Vt52cup(7, 47).T("nothing more."));

            s = t.GetScreenText();
            Assert.Equal(ExpectTestVt52ClearCup, s);

            // for (i = 1; i <= 10; i++)
            //     printf("THIS SHOULD GO AWAY! ");
            for (var i = 1; i <= 10; i++)
                Push(d, "THIS SHOULD GO AWAY! ");

            // for (i = 1; i <= 5; i++)
            // {
            //     vt52cup(1, 1);
            //     printf("%s", "Back scroll (this should go away)");
            //     vt52ri();   /* Reverse LineFeed (with backscroll!)  */
            // }
            for (var i = 1; i <= 5; i++)
            {
                Push(d, "".Vt52cup(1, 1).T("Back scroll (this should go away)").Vt52ri());
            }

            s = t.GetScreenText();
            Assert.Equal(ExpectTestVt52Backscroll, s);

            // vt52cup(12, 60);
            // vt52ed();     /* Erase to end of screen  */
            Push(d, "".Vt52cup(12, 60).Vt52ed());

            // for (i = 2; i <= 6; i++)
            // {
            //     vt52cup(i, 1);
            //     vt52el();   /* Erase to end of line */
            // }
            for (var i = 2; i <= 6; i++)
                Push(d, "".Vt52cup(i, 1).Vt52el());

            s = t.GetScreenText();
            Assert.Equal(ExpectTestVt52EraseToEnd, s);

            // for (i = 2; i <= max_lines - 1; i++)
            // {
            //     vt52cup(i, 70);
            //     printf("%s", "**Foobar");
            // }
            for (var i = 2; i <= MaxLines - 1; i++)
                Push(d, "".Vt52cup(i, 70).T("**Foobar"));

            // vt52cup(max_lines - 1, 10);
            Push(d, "".Vt52cup(MaxLines - 1, 10));

            // for (i = max_lines - 1; i >= 2; i--)
            // {
            //     printf("%s", "*");
            //     printf("%c", 8);  /* BS */
            //     vt52ri();   /* Reverse LineFeed (LineStarve)        */
            // }
            for (var i = MaxLines - 1; i >= 2; i--)
                Push(d, "*\b".Vt52ri());

            // vt52cup(1, 70);
            Push(d, "".Vt52cup(1, 70));

            // for (i = 70; i >= 10; i--)
            // {
            //     printf("%s", "*");
            //     vt52cub1();
            //     vt52cub1(); /* Cursor Left */
            // }
            for (var i = 70; i >= 10; i--)
                Push(d, "*".Vt52cub1().Vt52cub1());

            // vt52cup(max_lines, 10);
            Push(d, "".Vt52cup(MaxLines, 10));

            // for (i = 10; i <= 70; i++)
            // {
            //     printf("%s", "*");
            //     printf("%c", 8);  /* BS */
            //     vt52cuf1(); /* Cursor Right */
            // }
            for (var i = 10; i <= 70; i++)
                Push(d, "*\b".Vt52cuf1());

            // vt52cup(2, 11);
            Push(d, "".Vt52cup(2, 11));

            // for (i = 2; i <= max_lines - 1; i++)
            // {
            //     printf("%s", "!");
            //     printf("%c", 8);  /* BS */
            //     vt52cud1(); /* Cursor Down  */
            // }
            for (var i = 2; i <= MaxLines - 1; i++)
                Push(d, "!\b".Vt52cud1());

            // vt52cup(max_lines - 1, 69);
            Push(d, "".Vt52cup(MaxLines - 1, 69));

            // for (i = max_lines - 1; i >= 2; i--)
            // {
            //     printf("%s", "!");
            //     printf("%c", 8);  /* BS */
            //     vt52cuu1(); /* Cursor Up    */
            // }
            for (var i = MaxLines - 1; i >= 2; i--)
                Push(d, "!\b".Vt52cuu1());

            // for (i = 2; i <= max_lines - 1; i++)
            // {
            //     vt52cup(i, 71);
            //     vt52el();   /* Erase to end of line */
            // }
            for (var i = 2; i <= MaxLines - 1; i++)
                Push(d, "".Vt52cup(i, 71).Vt52el());

            // vt52cup(10, 16);
            // printf("%s", "The screen should be cleared, and have a centered");
            Push(d, "".Vt52cup(10, 16).T("The screen should be cleared, and have a centered"));

            // vt52cup(11, 16);
            // printf("%s", "rectangle of \"*\"s with \"!\"s on the inside to the");
            Push(d, "".Vt52cup(11, 16).T("rectangle of \"*\"s with \"!\"s on the inside to the"));

            // vt52cup(12, 16);
            // printf("%s", "left and right. Only this, and");
            Push(d, "".Vt52cup(12, 16).T("left and right. Only this, and"));

            // vt52cup(13, 16);
            Push(d, "".Vt52cup(13, 16));

            s = t.GetScreenText();
            Assert.Equal(ExpectTestVt52Final, s);
        }

        public static readonly Dictionary<string, string> ResponseTable = new Dictionary<string, string>
        {
            { "\u001b/A", " -- OK (VT50)" },
            { "\u001b/C", " -- OK (VT55)" },
            { "\u001b/H", " -- OK (VT50H without copier)" },
            { "\u001b/J", " -- OK (VT50H with copier)" },
            { "\u001b/K", " -- OK (means Standard VT52)" },
            { "\u001b/L", " -- OK (VT52 with copier)" },
            { "\u001b/Z", " -- OK (means VT100 emulating VT52)" },
        };

        [Fact]
        public void TextVt52Identify()
        {
            var t = new VirtualTerminalController();
            t.Debugging = true;
            var d = new DataConsumer(t);
            t.ResizeView(80, 25);

            string toSend = "";
            t.SendData += (sender, args) =>
            {
                toSend = Encoding.ASCII.GetString(args.Data);
            };

            // vt52home();   /* Cursor home     */
            // vt52ed();     /* Erase to end of screen  */
            // println("Test of terminal response to IDENTIFY command");
            Push(d, "".SetVt52Mode().Vt52Home().Vt52ed().T("Test of terminal response to IDENTIFY command\r\n"));

            /*
             * According to J.Altman, DECID isn't recognized by VT5xx terminals.  Real
             * DEC terminals through VT420 do, though it isn't recommended.  VT420's
             * emulation of VT52 does not recognize DA -- so we use DECID in this case.
             */
            // set_tty_raw(TRUE);
            // decid();      /* Identify     */
            // response = get_reply();
            // println("");
            Push(d, "".DECID().T("\r\n"));
            var response = toSend;

            // restore_level(&save);
            // restore_ttymodes();
            // padding(10);  /* some terminals miss part of the 'chrprint()' otherwise */
            Push(d, "".DECSCL(64, 0));

            // printf("Response was");
            // chrprint(response);
            Push(d, "Response was ".ChrPrint(response));


            // for (i = 0; resptable[i].rcode[0] != '\0'; i++)
            // {
            //     if (!strcmp(response, resptable[i].rcode))
            //     {
            //         show_result("%s", resptable[i].rmsg);
            //         break;
            //     }
            // }
            var found = ResponseTable.TryGetValue(response, out string responseText);
            Assert.True(found);

            Push(d, responseText);

            // println("");
            // println("");
            Push(d, "\r\n\r\n");

            // /*
            //  * Verify whether returning to ANSI mode restores the previous operating
            //  * level.  If it was a VT220, we can check this by seeing if 8-bit controls
            //  * work; if a VT420 we can check the value of DECSCL.  A real VT420 goes to
            //  * VT100 mode.
            //  */
            // if (terminal_id() >= 200)
            // {
            //     int row = 8;
            //     set_level(0);   /* Reset ANSI (VT100) mode, Set VT52 mode  */
            var row = 8;
            Push(d, "".SetVt52Mode());

            //     println("Verify operating level after restoring ANSI mode");
            //     esc("<");   /* Enter ANSI mode (VT100 mode) */
            Push(d, "".EnterANSIMode());

            //     set_tty_raw(TRUE);
            //     if (save.cur_level >= 3)
            //     {  /* VT340 implements DECRQSS */
            //         vt_move(row, 1);
            Push(d, "".CUP(row, 1));

            //         row = testing("DECSCL", row);
            //         println("You should have to press return to continue:");
            //         println("");
            //         decrqss("\"p");
            //         response = get_reply();
            toSend = "";
            Push(d, "Testing DECSL. A real VT420 will not recognize DECSL at this point\n".DECRQSS("\"p"));
            row++;

            Thread.Sleep(5);
            Assert.Empty(toSend);

            //         vt_move(++row, 10);
            //         printf("Response was");
            //         chrprint(response);
            //         if (isreturn(response))
            //         {
            //             show_result(SHOW_SUCCESS);
            //         }
            //         else
            //         {
            //             if (parse_decrqss(response, "\"p") > 0)
            //                 printf("DECSCL recognized --");
            //             show_result(SHOW_FAILURE);
            //         }
            //         println("");
            //         row++;
            //     }
            // 
            //     if (save.cur_level >= 2)
            //     {
            //         vt_move(++row, 1);
            //         row = testing("S8C1T", row);
            //         s8c1t(1);
            //         cup(1, 1);
            //         dsr(6);
            //         response = instr();
            //         vt_move(row, 10);
            //         printf("Response to CUP(1,1)/DSR(6)");
            //         chrprint(response);
            //         if ((temp = skip_prefix(csi_input(), response)) != 0)
            //         {
            //             if (!strcmp("1;1R", temp))
            //             {
            //                 printf("S8C1T recognized --");
            //                 show_result(SHOW_FAILURE);
            //             }
            //             else
            //             {
            //                 printf("unknown response --");
            //                 show_result(SHOW_FAILURE);
            //             }
            //         }
            //         else
            //         {
            //             input_8bits = FALSE;  /* we expect this anyway */
            //             if ((temp = skip_prefix(csi_input(), response)) != 0
            //                 && !strcmp("1;1R", temp))
            //             {
            //                 show_result(SHOW_SUCCESS);
            //             }
            //             else
            //             {
            //                 printf("unknown response --");
            //                 show_result(SHOW_FAILURE);
            //             }
            //         }
            //     }
            //     restore_level(&save);
            //     restore_ttymodes();
            //     println("");
            //     println("");
            // }
        }
    }
}
