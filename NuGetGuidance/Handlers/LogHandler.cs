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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using NuGetGuidance.Domain;
using NuGetGuidance.Interfaces;
using NuGetGuidance.Properties;

namespace NuGetGuidance.Handlers
{
    internal class LogHandler : ILogHandler
    {
        public ObservableCollection<ILogMessage> LogMessages { get; private set; }

        public LogHandler()
        {
            LogMessages = new ObservableCollection<ILogMessage>();
        }

        public ILogHandler Separate()
        {
            var ruler = new LogSeparator();
            AddMessage(ruler);

            return this;
        }

        public ILogHandler Log(string message, params string[] arguments)
        {
            var formattedMessage = string.Format(CultureInfo.CurrentCulture, message, arguments);

            var logMessage = new LogMessage(formattedMessage);
            AddMessage(logMessage);

            return this;
        }

        public ILogHandler Log(string message, LogLevel logLevel, params string[] arguments)
        {
            var formattedMessage = string.Format(CultureInfo.CurrentCulture, message, arguments);
            var logMessage = new LogMessage(formattedMessage, logLevel);
            AddMessage(logMessage);

            return this;
        }

        public ILogHandler LogException(Exception exception)
        {
            try
            {
                var formattedException = FormatException(exception);
                var formattedMessage = string.Format(CultureInfo.CurrentCulture, Resources.Exception, exception.Message);
                var logMessage = new ExceptionLogMessage(formattedMessage, formattedException);

                AddMessage(logMessage);

                Log(Resources.EventViewer, LogLevel.Error);
                Log(Resources.Help, LogLevel.Error);

                string application = Assembly.GetEntryAssembly().GetName().Name;

                if (!EventLog.SourceExists(application))
                    EventLog.CreateEventSource(application, "Application");

                using (var eventLog = new EventLog {Source = application})
                {
                    eventLog.WriteEntry(formattedException, EventLogEntryType.Error);
                }
            }
            catch
            {
                //Something went wrong trying to log, can't do much about this...
                //Either catch all or crash, which logging isn't worth.
            }

            return this;
        }

        private static string FormatException(Exception exception)
        {
            var builder = new StringBuilder();
            builder.AppendFormat(CultureInfo.CurrentCulture, Resources.ExceptionInfo, exception.Message);

            if (exception.InnerException != null)
            {
                builder.AppendLine()
                       .AppendLine()
                       .AppendLine()
                       .Append(Resources.ErrorDetail)
                       .Append(":")
                       .AppendLine()
                       .AppendLine()
                       .Append("\"");

                FormatExceptionDetail(exception.InnerException, builder);

            }

            builder.AppendLine("\"")
                   .AppendLine()
                   .AppendLine("StackTrace:")
                   .AppendLine()
                   .Append(exception.StackTrace);

            return builder.ToString();
        }
        
        private static void FormatExceptionDetail(Exception exception, StringBuilder builder)
        {
            builder.Append(exception.Message);

            if (exception.InnerException == null) return;

            builder.Append(", ")
                   .AppendLine()
                   .AppendLine();

            FormatExceptionDetail(exception.InnerException, builder);
        }

        private void AddMessage(ILogMessage message)
        {
            var dispatcher = Application.Current.Dispatcher;

            if (dispatcher.CheckAccess())
            {
                LogMessages.Add(message);
            }
            else
            {
                dispatcher.BeginInvoke(new Action(() => LogMessages.Add(message)), DispatcherPriority.Send);
            }
        }

        public void Save(string fileName)
        {
            var builder = new StringBuilder(LogMessages.Count);

            foreach (var logMessage in LogMessages)
            {
                builder.AppendLine(logMessage.ToString());
            }

            var contents = builder.ToString();
            File.WriteAllText(fileName, contents);
        }
    }
}
