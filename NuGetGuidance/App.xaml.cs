using System;
using System.IO;
using System.Windows;
using NuGetGuidance.Handlers;

namespace NuGetGuidance
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public bool InitError { get; private set; }
        internal FileInfo Project { get; private set; }
        internal FileInfo Solution { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
#if DEBUG
            DispatcherUnhandledException += (sender, args) => Debugger.Break();
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => Debugger.Break();
#endif

            DispatcherUnhandledException += ExceptionHandler.DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += ExceptionHandler.UnhandledException;

            base.OnStartup(e);

            try
            {
                ParseStartupArguments(e);
            }
            catch
            {
                InitError = true;
            }

            if (!InitError)
            {
                //Creating Catalogs in advance in the threadpool as this takes longest.
                RecipeCompositionHandler.StartComposing();
            }
        }

        private void ParseStartupArguments(StartupEventArgs e)
        {
            var arguments = e.Args;

            if (arguments.Length != 2)
            {
                InitError = true;
                return;
            }
            
            var project = arguments[0];
            if (IsValid(project))
            {
                var fileName = project.Trim();
                Project = new FileInfo(fileName);
            }

            var solution = arguments[1];
            if (IsValid(solution))
            {
                var fileName = solution.Trim();
                Solution = new FileInfo(fileName);
            }
        }

        private bool IsValid(string argument)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                InitError = true;
                return false;
            }

            return true;
        }
    }
}
