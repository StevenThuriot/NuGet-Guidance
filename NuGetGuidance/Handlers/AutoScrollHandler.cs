using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace NuGetGuidance.Handlers
{
    internal class AutoScrollHandler
    {
        public static readonly DependencyProperty AutoScrollToEndProperty = DependencyProperty.RegisterAttached("AutoScrollToEnd", typeof(bool), typeof(AutoScrollHandler), new UIPropertyMetadata(default(bool), OnAutoScrollToEndChanged));


        public static bool GetAutoScrollToEnd(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToEndProperty);
        }

        public static void SetAutoScrollToEnd(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToEndProperty, value);
        }

        public static void OnAutoScrollToEndChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool) e.NewValue)
                return;

            var listBox = s as ListBox;
            
            if (listBox == null) return;

            var data = listBox.Items.SourceCollection as INotifyCollectionChanged;
            
            if (data == null) return;

            data.CollectionChanged += (sender, args) =>
                {
                    var items = sender as ItemCollection;

                    if (items == null || items.Count == 0) return;

                    var lastItem = items[items.Count - 1];
                    items.MoveCurrentTo(lastItem);
                    listBox.ScrollIntoView(lastItem);
                };
        }
    }
}
