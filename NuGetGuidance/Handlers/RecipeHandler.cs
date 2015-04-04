using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using NuGetGuidance.Domain;
using NuGetGuidance.Interfaces;
using RESX=NuGetGuidance.Properties.Resources;

namespace NuGetGuidance.Handlers
{
    [Export]
    internal class RecipeHandler : IRunnable
    {
#pragma warning disable 0649
        [ImportMany]
        private IEnumerable<IRecipe> _Recipes;
        
        [Import]
        private ILogHandler _Logger;
#pragma warning restore 0649

        public int Count
        {
            get
            {
                return _Recipes == null
                           ? 0
                           : _Recipes.Count();
            }
        }

        public Task<bool> Run()
        {
            return Task.Run(() => Handle());
        }

        private async Task<bool> Handle()
        {
            var succeeded = true;

            if (_Recipes != null && _Recipes.Any())
            {
                _Logger.Separate();

                try
                {
                    foreach (var recipe in _Recipes.OrderBy(x => x.Priority))
                    {
                        if (await recipe.Run()) continue;

                        succeeded = false;
                        break;
                    }
                }
                catch
                {
                    succeeded = false;
                    _Logger.Log(RESX.UnexpectedError, LogLevel.Error);
                }

                if (succeeded)
                {
                    _Logger.Log(RESX.RunSuccessful);
                }
                else
                {
                    _Logger.Log(RESX.RunFailed, LogLevel.Error);
                }
            }
            else
            {
                _Logger.Log(RESX.NoRecipesFound, LogLevel.Warning);
                succeeded = false;
            }

            await Task.Delay(500);

            return succeeded;
        }
    }
}
