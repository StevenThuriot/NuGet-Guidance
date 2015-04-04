using NuGetGuidance.Domain;

namespace NuGetGuidance.Interfaces
{
    public interface ILogMessage
    {
        LogLevel LogLevel { get; }
        string Message { get; }
    }
}