namespace NuGetGuidance.Interfaces
{
    public interface IPromptResult
    {
        bool Result { get; }
        string Input { get; }
        bool IsOpen { get; }
    }
}