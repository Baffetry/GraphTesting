using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Task_Panel_Drawer
{
    class TaskDockPanelCreator : IDockPanelCreator
    {
        private IObserve? observe;

        public DockPanel GetDockPanel(string labelContent, int index)
        {
            observe = ObserveCheckBox.GetObserver();

            var panel = new DockPanel();
            panel.Margin = new Thickness(0, 10, 0, 10);


            var label = new Label
            {
                Content = $"{index + 1}. {labelContent}",
                Width = 750
            };

            var checkBox = new CheckBox
            {
                Name = $"Task{index + 1}",
            };


            int obsIdx = observe.FindBox(checkBox);
            if (observe != null && obsIdx != -1)
                checkBox.IsChecked = observe.IsChecked(observe.FindBox(checkBox));


            checkBox.Checked += CheckBox_Checked;
            checkBox.Unchecked += CheckBox_Unchecked;


            
            DockPanel.SetDock(label, Dock.Left);
            
            observe?.Add(checkBox);
            panel.Children.Add(label);
            panel.Children.Add(checkBox);

            return panel;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                observe?.Update(checkBox);
                checkBox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4dd49e"));
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                observe?.Update(checkBox);
                checkBox.Background = Brushes.Transparent;
            }
        }
    }
}