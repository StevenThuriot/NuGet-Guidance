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
using System.Diagnostics;
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
