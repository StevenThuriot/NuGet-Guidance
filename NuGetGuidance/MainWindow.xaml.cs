using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;
using NuGetGuidance.Domain;
using NuGetGuidance.Handlers;
using RESX = NuGetGuidance.Properties.Resources;

namespace NuGetGuidance
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    internal partial class MainWindow : INotifyPropertyChanged
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public LogHandler Logger { get; private set; }

        private bool _IsExecuting;
        public bool IsExecuting
        {
            get { return _IsExecuting; }
            set
            {
                if (_IsExecuting != value)
                {
                    _IsExecuting = value;
                    OnPropertyChanged("IsExecuting");
                }
            }
        }

        private bool _IsInError;
        public bool IsInError
        {
            get { return _IsInError; }
            set
            {
                if (_IsInError != value)
                {
                    _IsInError = value;
                    OnPropertyChanged("IsInError");
                }
            }
        }

        public MainWindow()
        {
            _IsExecuting = false;

            Logger = new LogHandler();

            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
            VisualTextRenderingMode = TextRenderingMode.ClearType;

            DataContext = this;

            InitializeComponent();

            Loaded += WindowLoaded;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= WindowLoaded;

            Task.Run(() => TryToSetParent(this));

            Run();
        }

        private async void Run()
        {
            var app = (App)Application.Current;
            if (app.InitError)
            {
                Logger.Log(RESX.StartUpErrorMessage, LogLevel.Error);
                IsInError = true;
                return;
            }

            IsExecuting = true;

            Logger.Log(RESX.StartUp)
                  .Separate()
                  .Log(Properties.Resources.ComposingRecipes);

            await Task.Delay(800);

            var handler = await RecipeCompositionHandler.GenerateHandler(Logger, Prompt);

            Logger.Log(RESX.ComposedRecipes, handler.Count.ToString(CultureInfo.CurrentCulture));

            var result = await handler.Run();

            IsExecuting = false;
            IsInError = !result;
        }

        private static void TryToSetParent(Window window)
        {
            if (window == null) return;

            var hwnd = FindWindow(null, "Installing..."); //Installing new NuGet.

            if (hwnd == IntPtr.Zero)
            {
                hwnd = FindWindow(null, "Updating..."); //Updating existing NuGet.
                if (hwnd == IntPtr.Zero) return;
            }

            var windowInteropHelper = new WindowInteropHelper(window);

            windowInteropHelper.Owner = hwnd;
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            if (IsExecuting)
            {
                var dialog = MessageBox.Show(RESX.ShutdownDuringExecution, RESX.Caption, MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (dialog == MessageBoxResult.No)
                    return;
            }

            Application.Current.Shutdown();
        }

        private void SaveLog(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                DefaultExt = "txt",
                Filter = "Text File|*.txt",
                FilterIndex = 1,
                AddExtension = true,
                CreatePrompt = false,
                OverwritePrompt = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
            };
            
            if (!saveFileDialog.ShowDialog().GetValueOrDefault()) return;

            Logger.Save(saveFileDialog.FileName);
        }









        private PromptResult _PromptResult;
        public PromptResult PromptResult
        {
            get { return _PromptResult; }
            set
            {
                if (_PromptResult != value)
                {
                    _PromptResult = value;
                    OnPropertyChanged("PromptResult");
                }
            }
        }

        public PromptResult Prompt(string question, string input)
        {
            Dispatcher.BeginInvoke(new Action(() => { _PromptGrid.Visibility = Visibility.Visible; }), DispatcherPriority.Input);

            return PromptResult = new PromptResult(question, input);
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            PromptResult.Result = true; 
            _PromptGrid.Visibility = Visibility.Hidden;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            PromptResult.Result = false;
            _PromptGrid.Visibility = Visibility.Hidden;
        }
    }
}
