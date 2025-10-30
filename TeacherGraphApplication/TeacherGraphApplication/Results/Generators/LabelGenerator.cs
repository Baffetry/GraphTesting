using System.Windows.Controls;
using System.Windows;

namespace Generators
{
    public class LabelGenerator
    {
        public Label GenerateLabel(object? content)
        {
            return new Label
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = content,
                FontWeight = FontWeights.Bold
            };
        }
    }
}
