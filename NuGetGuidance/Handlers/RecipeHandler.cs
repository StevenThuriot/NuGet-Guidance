#region License

// 
//  Copyright 2012 Steven Thuriot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 

#endregion

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
        [ImportMany]
        private IEnumerable<IRecipe> _Recipes;
        
        [Import]
        private ILogHandler _Logger;

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
