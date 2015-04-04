using System;
using System.Globalization;

namespace NuGetGuidance.Domain
{
    internal class ExceptionLogMessage : LogMessage
    {
        public ExceptionLogMessage(string message, string exception)
            : base(message, LogLevel.Error)
        {
            Exception = exception;
        }

        public string Exception { get; private set; }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0},{1} • Exception: [ {2} ]", base.ToString(), Environment.NewLine, Exception);
        }
    }
}