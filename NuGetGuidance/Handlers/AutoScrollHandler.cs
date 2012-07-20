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
