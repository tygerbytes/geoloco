using System;
using System.IO;
using GeoLoco.Core.Interfaces;

namespace GeoLoco.Infrastructure.Services
{
    public class FileLogger : IAppLogger
    {
        private readonly string path;
        private readonly IDateService date;
        private readonly bool verbose;

        public FileLogger(string path, IDateService dateService, bool wipe = false, bool verbose = false)
        {
            this.path = path;
            date = dateService;
            this.verbose = verbose;
            if (wipe)
            {
                Wipe();
            }
        }

        public void LogError(string message)
        {
            Log(message, "❌");
        }

        public void LogInformation(string message)
        {
            Log(message, "💡");
        }

        public void LogVerbose(string message)
        {
            if (verbose)
            {
                Log(message, "🎤");
            }
        }

        public void LogWarning(string message)
        {
            Log(message, "⚠");
        }

        private void Log(string message, string icon)
        {
            using var streamWriter = File.AppendText(path);
            streamWriter.WriteLine($"[{date.Now}] {icon} \"{message}\"");
        }

        private void Wipe()
        {
            try
            {
                File.Delete(path);
                Log(string.Empty , "");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error initializing log file '{path}': {ex.Message}");
            }
        }
    }
}
