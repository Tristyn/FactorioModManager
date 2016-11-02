using System;
using System.ComponentModel;
using Splat;

namespace FactorioModManager.UI
{
    class ConsoleLogger : ILogger
    {
        public ConsoleLogger()
        {
            Level = LogLevel.Debug;
        }

        public LogLevel Level { get; set; }

        public void Write([Localizable(false)] string message, LogLevel logLevel)
        {
            Console.WriteLine("{0}: {1}", logLevel, message);
        }
    }
}
