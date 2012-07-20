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
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using NuGetGuidance.Interfaces;

namespace NuGetGuidance.Handlers
{
    internal static class RecipeCompositionHandler
    {
        private static Task<CompositionContainer> _Container;

        public static void StartComposing()
        {
            if (_Container != null) return;

            var compose = new Func<CompositionContainer>(ComposeContainer);
            _Container = Task.Run(compose);
        }

        public async static Task<CompositionContainer> Compose()
        {
            StartComposing();
            return await _Container;
        }

        private static CompositionContainer ComposeContainer()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var assemblyCatalog = new AssemblyCatalog(assembly);

            var executable = new FileInfo(assembly.Location);
            var directoryCatalog = new DirectoryCatalog(executable.DirectoryName, "*.dll");
            var aggregateCatalog = new AggregateCatalog(assemblyCatalog, directoryCatalog);

            AddChildDirectories(executable.Directory, aggregateCatalog);

            var container = new CompositionContainer(aggregateCatalog);
            
            return container;
        }

        private static void AddChildDirectories(DirectoryInfo appDirectory, AggregateCatalog aggregateCatalog)
        {
            foreach (var childDirectory in appDirectory.GetDirectories())
            {
                AddChildDirectories(childDirectory, aggregateCatalog);

                var childDirectoryCatalog = new DirectoryCatalog(childDirectory.FullName, "*.dll");
                aggregateCatalog.Catalogs.Add(childDirectoryCatalog);
            }
        }

        public static async Task<RecipeHandler> GenerateHandler(ILogHandler logger, Func<string, string, IPromptResult> prompt)
        {
            var container = await Compose();

            container.ComposeExportedValue(logger);
            container.ComposeExportedValue(prompt);

            var app = (App)Application.Current;

            container.ComposeExportedValue("Project", app.Project);
            container.ComposeExportedValue("Solution", app.Solution);

            var handler = container.GetExportedValue<RecipeHandler>();
            return handler;
        }
    }
}
