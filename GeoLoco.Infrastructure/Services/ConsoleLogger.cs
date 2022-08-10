using System;
using GeoLoco.Core.Interfaces;

namespace GeoLoco.Infrastructure.Services
{
    public class ConsoleLogger : IAppLogger
    {
        private readonly bool verbose;

        public ConsoleLogger(bool verbose)
        {
            this.verbose = verbose;
        }

        public void LogError(string message)
        {
            Console.WriteLine($"❌{message}");
        }

        public void LogInformation(string message)
        {
            Console.WriteLine($"💡{message}");
        }

        public void LogVerbose(string message)
        {
            if (verbose)
            {
                Console.WriteLine($"{message}");
            }
        }

        public void LogWarning(string message)
        {
            Console.WriteLine($"⚠{message}");
        }
    }
}
