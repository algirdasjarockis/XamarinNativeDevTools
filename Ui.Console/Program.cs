using System;
using XamarinNativeDevTools;

namespace UiConsole
{
    internal class Program
    {

        static void Main(string[] args)
        {
            var handler = new ArgumentHandler(args);
            handler.ValidateArgs();

            if (handler.Command == ArgumentHandler.CommandResourceUsage)
            {
                var resourceFinder = new ResourceUsageScanner();
                resourceFinder.ScanSolutionDirectory(handler.CommandArgument);
            }
            else
                Console.WriteLine("Did nothing");
        }

        private class ArgumentHandler
        {
            public const string CommandResourceUsage = "resource_usage";

            private string[] AllowableCommands = { CommandResourceUsage };

            private string[] _args;

            public ArgumentHandler(string[] args) { _args = args; }

            public string Command => _args[0];

            public string CommandArgument => _args[1];

            public void ValidateArgs()
            {

                if (_args.Length < 2)
                    throw new ArgumentException("Not enough amount of arguments was provided");

                if (!AllowableCommands.Any(_args[0].Equals))
                    throw new ArgumentException($"Unknown command '{_args[0]}' given");
            }
        }
    }
}