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
        private static Task<AggregateCatalog> _Catalog;

        public static void StartComposing()
        {
            if (_Catalog != null) return;

            var compose = new Func<AggregateCatalog>(ComposeCatalogs);
            _Catalog = Task.Run(compose);
        }

        private static AggregateCatalog ComposeCatalogs()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var assemblyCatalog = new AssemblyCatalog(assembly);

            var executable = new FileInfo(assembly.Location);
            var directoryCatalog = new DirectoryCatalog(executable.DirectoryName, "*.dll");
            var aggregateCatalog = new AggregateCatalog(assemblyCatalog, directoryCatalog);

            AddChildDirectories(executable.Directory, aggregateCatalog);

            return aggregateCatalog;
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
            StartComposing();
            var catalog = await _Catalog;

            var container = new CompositionContainer(catalog);

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
