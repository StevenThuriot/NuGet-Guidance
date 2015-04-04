using NuGetGuidance.Interfaces;

namespace NuGetGuidance.Domain
{
    internal class LogSeparator : ILogMessage
    {
        public LogLevel LogLevel
        {
            get { return LogLevel.Information; }
        }

        public string Message
        {
            get { return Properties.Resources.LogSeperator; }
        }

        public override string ToString()
        {
            return " " + Message;
        }
    }
}