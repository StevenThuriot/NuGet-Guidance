using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace NuGetGuidance
{
    public partial class ProgressIndicator
    {
        public  ProgressIndicator()
        {
            InitializeComponent();
            DataContext = this;
            IsVisibleChanged += StartStopAnimation;
        }

        private void StartStopAnimation(object sender, DependencyPropertyChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
                {
                    var storyboard = (Storyboard) Resources["animate"];

                    if ((bool) e.NewValue)
                        storyboard.Begin();
                    else
                        storyboard.Stop();

                })
                );
        }
    }
}
