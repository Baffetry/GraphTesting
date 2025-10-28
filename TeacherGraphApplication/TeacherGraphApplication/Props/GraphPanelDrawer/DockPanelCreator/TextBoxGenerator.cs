using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TeacherGraphApplication.Props.GraphPanelDrawer.DockPanelCreator
{
    public class TextBoxGenerator
    {
        public TextBox GenerateTextBox(int index)
        {
            var textBox = new TextBox
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                MaxLength = 2,
                Margin = new Thickness(20, 0, 20, 0),
                Name = $"TextBox{index}"
            };

            textBox.PreviewTextInput += TextBox_PreviewTextInput;

            return textBox;
        }
        
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }
    }
}
