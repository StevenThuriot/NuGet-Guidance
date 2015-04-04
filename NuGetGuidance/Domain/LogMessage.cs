using System.Globalization;
using NuGetGuidance.Interfaces;

namespace NuGetGuidance.Domain
{
    internal class LogMessage : ILogMessage
    {
        public LogMessage(string message)
            : this (message, LogLevel.Information)
        {
        }

        public LogMessage(string message, LogLevel logLevel)
        {
            Message = message;
            LogLevel = logLevel;
        }

        public LogLevel LogLevel { get; private set; }
        public string Message { get; private set; }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, Properties.Resources.LogMessage, Message, LogLevel);
        }
    }
}
