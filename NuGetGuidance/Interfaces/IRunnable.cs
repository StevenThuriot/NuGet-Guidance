using System.Threading.Tasks;

namespace NuGetGuidance.Interfaces
{
    public interface IRunnable
    {
        /// <summary>
        /// Runs this instance.
        /// </summary>
        /// <returns></returns>
        Task<bool> Run();
    }
}
