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
