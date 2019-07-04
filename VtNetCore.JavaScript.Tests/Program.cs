using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using System;
using System.IO;
using System.Linq;
using VtNetCore.VirtualTerminal;
using VtNetCore.XTermParser;

namespace VtNetCore.JavaScript.Tests
{
    public static class Helpers
    {
        public static string ToSequenceString(this byte [] seq)
        {
            var str = "";
            for (var i = 0; i < seq.Length; i++)
            {
                switch (seq[i])
                {
                    case 0: str += "<nil>"; break;
                    case 1: str += "<soh>"; break;
                    case 2: str += "<stx>"; break;
                    case 3: str += "<etx>"; break;
                    case 4: str += "<eot>"; break;
                    case 5: str += "<enq>"; break;
                    case 6: str += "<ack>"; break;
                    case 7: str += "<bel>"; break;
                    case 8: str += "<bs>"; break;
                    case 9: str += "<ht>"; break;
                    case 10: str += "<lf>"; break;
                    case 11: str += "<vt>"; break;
                    case 12: str += "<ff>"; break;
                    case 13: str += "<cr>"; break;
                    case 14: str += "<si>"; break;
                    case 15: str += "<so>"; break;
                    case 16: str += "<dle>"; break;
                    case 17: str += "<dc1>"; break;
                    case 18: str += "<dc2>"; break;
                    case 19: str += "<dc3>"; break;
                    case 20: str += "<dc4>"; break;
                    case 21: str += "<nak>"; break;
                    case 22: str += "<syn>"; break;
                    case 23: str += "<etb>"; break;
                    case 24: str += "<can>"; break;
                    case 25: str += "<em>"; break;
                    case 26: str += "<sub>"; break;
                    case 27: str += "<esc>"; break;
                    case 28: str += "<fs>"; break;
                    case 29: str += "<gs>"; break;
                    case 30: str += "<rs>"; break;
                    case 31: str += "<us>"; break;
                    case 127: str += "<del>"; break;
                    default:
                        if (seq[i] < 128)
                        {
                            str += (char)seq[i];
                        }
                        else
                        {
                            str += "{" + seq[i].ToString("X2") + "}";
                        }
                        break;
                }
            }
            return str;
        }
    }

    public class Terminal
    {
        VirtualTerminalController Controller;
        DataConsumer Consumer;

        private byte[] SendBuffer = new byte[0];

        public Terminal()
        {
            Controller = new VirtualTerminalController();
            Consumer = new DataConsumer(Controller);
            Controller.ResizeView(80, 25);

            Controller.SendData += TerminalSendDataEvent;
        }

        private void TerminalSendDataEvent(object sender, SendDataEventArgs e)
        {
            SendBuffer = SendBuffer.Concat(e.Data).ToArray();
        }

        [ScriptMember("push")]
        public void Push(ITypedArray data)
        {
            var rawData = data.GetBytes();
            Consumer.Push(rawData);
        }

        [ScriptMember("screenText")]
        public string ScreenText { get => Controller.GetScreenText(); }

        [ScriptMember("column")]
        public int Column { get => Controller.CursorState.CurrentColumn; }

        [ScriptMember("row")]
        public int Row { get => Controller.CursorState.CurrentRow; }

        [ScriptMember("resizeView")]
        public void ResizeView(int columns, int rows) => Controller.ResizeView(columns, rows);

        [ScriptMember("getSendBuffer")]
        public string GetSendBuffer()
        {
            var result = SendBuffer;
            SendBuffer = new byte[0];
            return result.ToSequenceString();
        }
    }

    public class Log
    {
        public Log()
        {
        }

        [ScriptMember("info")]
        public void WriteInformational(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Info: ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(text);
        }

        [ScriptMember("dump")]
        public void WriteDump(string text)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Dump: ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(text);
        }

        [ScriptMember("debug")]
        public void WriteDebug(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Debug: ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(text);
        }

        [ScriptMember("status")]
        public void WriteStatus(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Status: ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(text);
        }
    }

    class TestRunner
    {
        private V8ScriptEngine Engine;

        public TestRunner()
        {
            Engine = new V8ScriptEngine();
            Engine.AddHostType("Terminal", typeof(Terminal));
            Engine.AddHostObject("host", new HostFunctions());
            Engine.AddHostObject("log", new Log());

            foreach (string file in Directory.EnumerateFiles("Libraries", "*.js", SearchOption.AllDirectories))
            {
                Console.WriteLine($"Found library: {file}");
                var scriptText = File.ReadAllText(file);
                Engine.Execute(scriptText);
            }
        }

        public bool Run()
        {
            int testsTried = 0;
            int testsRun = 0;
            int testsPassed = 0;

            foreach (string file in Directory.EnumerateFiles("Tests", "*.js", SearchOption.AllDirectories))
            {
                Console.WriteLine($"-------------------------------");
                testsTried++;

                Console.WriteLine($"Found: {file}");
                var scriptText = File.ReadAllText(file);

                try
                {
                    Engine.Execute(scriptText);
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Script threw exception: {e.Message}");
                    continue;
                }

                try
                {
                    var x = Engine.Script.getTestInformation();
                    Console.WriteLine($"Name: {x.name}");
                    Console.WriteLine($"Features: {x.features}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"getTestInformation threw exception: {e.Message}");
                    continue;
                }

                try
                {
                    testsRun++;

                    var ok = Engine.Script.executeTest();
                    if (ok)
                        testsPassed++;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"executeTest threw exception: {e.Message}");
                    continue;
                }
            }

            Console.WriteLine($"-------------------------------");
            Console.WriteLine($"Tried: {testsTried}");
            Console.WriteLine($"Run: {testsRun}");
            Console.WriteLine($"Passed: {testsPassed}");
            Console.WriteLine($"Failed: {testsTried - testsPassed}");

            return (testsTried - testsPassed) == 0;
        }
    }

    class Program
    {
        static int Main(string[] args)
        {
            var t = new TestRunner();
            var passed = t.Run();

            Console.ReadKey();

            return passed ? 0 : -1;
        }
    }
}
