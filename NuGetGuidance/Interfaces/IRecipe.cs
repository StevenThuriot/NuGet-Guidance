using System.ComponentModel.Composition;

namespace NuGetGuidance.Interfaces
{
    [InheritedExport]
    public interface IRecipe : IRunnable
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the goal of this recipe.
        /// </summary>
        /// <value>
        /// The goal.
        /// </value>
        string Goal { get; }

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <remarks>Lower values will be run first.</remarks>
        /// <value>
        /// The priority.
        /// </value>
        int Priority { get; }
    }
}