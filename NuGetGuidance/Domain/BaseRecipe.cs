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

using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NuGetGuidance.Interfaces;
using NuGetGuidance.Properties;

namespace NuGetGuidance.Domain
{
    public abstract class BaseRecipe : IRecipe
    {
        [Import]
        private Func<string, string, IPromptResult> _Prompt;

        [Import]
        protected ILogHandler Log { get; private set; }

        [Import("Solution")]
        protected FileInfo Solution { get; private set; }

        [Import("Project")]
        protected FileInfo Project { get; private set; }

        public abstract string Name { get; }
        public abstract string Goal { get; }

        public async Task<bool> Run()
        {            
            Log.Log(Resources.StartingRecipe, Name);

            bool succeeded;
            try
            {
                var execute = new Func<bool>(Execute);

                succeeded = await Task.Run(execute);
                await Task.Delay(500);
            }
            catch (Exception e)
            {
                succeeded = false;
                Log.LogException(e);
            }

            if (succeeded)
            {
                Log.Log(Resources.Succeeded, LogLevel.Success, Goal);
                Log.Log(Resources.FinishingRecipe, Name);
            }
            else
            {
                Log.Log(Resources.Failed, LogLevel.Error, Goal);
            }

            Log.Separate();

            return succeeded;
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        protected abstract bool Execute();

        public virtual int Priority
        {
            get { return 50; }
        }

        /// <summary>
        /// Prompts for a specified value.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        protected IPromptResult Prompt(string field, string input = "")
        {
            Thread.Sleep(500);

            var question = string.Format(CultureInfo.InvariantCulture, Resources.PleaseSupply, field);
            var promptResult = _Prompt(question, input) ?? PromptResult.Empty;

            while (promptResult.IsOpen)
            {
                //Wait for input to finish.
                Thread.Sleep(250);
            }

            return promptResult;
        }
    }
}