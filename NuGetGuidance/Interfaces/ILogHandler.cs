using System;
using System.Collections.ObjectModel;
using NuGetGuidance.Domain;

namespace NuGetGuidance.Interfaces
{
    public interface ILogHandler
    {
        ObservableCollection<ILogMessage> LogMessages { get; }
        ILogHandler Separate();
        ILogHandler Log(string message, params string[] arguments);
        ILogHandler Log(string message, LogLevel logLevel, params string[] arguments);
        ILogHandler LogException(Exception exception);
        void Save(string fileName);
    }
}