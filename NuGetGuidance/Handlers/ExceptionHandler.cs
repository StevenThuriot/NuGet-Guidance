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
using System.Windows;
using System.Windows.Threading;

namespace NuGetGuidance.Handlers
{
    internal class ExceptionHandler
    {
        internal static void DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            SendToLogger(e.Exception);
            e.Handled = true;

            HandleWindow();
        }

        internal static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                SendToLogger(exception);
            }

            HandleWindow();
        }

        private static void SendToLogger(Exception exception)
        {
            var window = (MainWindow)Application.Current.MainWindow;
            window.Logger.LogException(exception);
        }

        private static void HandleWindow()
        {
            var window = Application.Current.MainWindow as MainWindow;
            if (window == null) return;

            window.IsExecuting = false;
            window.IsInError = true;
        }
    }
}
