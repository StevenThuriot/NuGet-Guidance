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
#pragma warning disable 0649
        [Import]
        private Func<string, string, IPromptResult> _Prompt;
#pragma warning restore 0649

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