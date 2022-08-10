using System;
using System.Collections.Generic;
using GeoLoco.Core.Interfaces;

namespace GeoLoco.Infrastructure.Services
{
    public class AggregateLogger : IAppLogger
    {
        private readonly IEnumerable<IAppLogger> loggers;

        public AggregateLogger(IEnumerable<IAppLogger> loggers)
        {
            this.loggers = loggers;
        }

        public void LogError(string message)
        {
            Log(l => l.LogError(message));
        }

        public void LogInformation(string message)
        {
            Log(l => l.LogInformation(message));
        }

        public void LogVerbose(string message)
        {
            Log(l => l.LogVerbose(message));
        }

        public void LogWarning(string message)
        {
            Log(l => l.LogWarning(message));
        }

        private void Log(Action<IAppLogger> action)
        {
            foreach (var logger in loggers)
            {
                action.Invoke(logger);
            }
        }
    }
}
