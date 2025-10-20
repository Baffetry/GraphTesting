using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Graph_Panel_Drawer
{
    class GraphDockPanelCreator : IDockPanelCreator
    {
        public DockPanel GetDockPanel(string labelContent)
        {
            var panel = new DockPanel();

            panel.Margin = new Thickness(0, 10, 0, 10);

            var label = new Label
            {
                Content = labelContent
            };

            var textBox = new TextBox
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                MaxLength = 2,
                Margin = new Thickness(20, 0, 20, 0)
            };

            textBox.PreviewTextInput += TextBox_PreviewTextInput;

            panel.Children.Add(label);
            panel.Children.Add(textBox);

            return panel;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }
    }
}