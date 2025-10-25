using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TeacherGraphApplication.Props.TaskPanelDrawer.Generators
{
    internal class CheckBoxGenerator
    {
        public CheckBox GenerateCheckBox(int index)
        {
            var checkBox = new CheckBox { Name = $"Task{index}" };

            checkBox.Checked += CheckBox_Checked;
            checkBox.Unchecked += CheckBox_Unchecked;

            return checkBox;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
                checkBox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4dd49e"));
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
                checkBox.Background = Brushes.Transparent;
        }
    }
}
