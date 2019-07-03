using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using System;
using System.IO;
using VtNetCore.VirtualTerminal;
using VtNetCore.XTermParser;

namespace VtNetCore.JavaScript.Tests
{

    public class Terminal
    {
        VirtualTerminalController Controller;
        DataConsumer Consumer;

        public Terminal()
        {
            Controller = new VirtualTerminalController();
            Consumer = new DataConsumer(Controller);
            Controller.ResizeView(80, 25);
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
