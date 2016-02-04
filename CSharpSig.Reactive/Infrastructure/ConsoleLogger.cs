using System;
using Splat;

namespace ImprovingU.Reactive
{
    public class ConsoleLogger : ILogger
    {
        public LogLevel Level { get; set; }

        public void Write(string message, LogLevel logLevel)
        {
            if (logLevel < Level) return;

            Console.WriteLine(message);
        }
    }
}