using System.Windows.Controls;
using System.Windows;

namespace Generators
{
    public class BorderGenerator()
    {
        private LabelGenerator generator = new LabelGenerator();
        public int MaxIndex {  get; set; }

        public Border GenerateBorder(object? content, int rowIndex)
        {
            return new Border 
            { 
                Child = generator.GenerateLabel(content),
                Margin = new Thickness(-1),
                CornerRadius = GetCornerRadius(rowIndex)
            };
        }

        private CornerRadius GetCornerRadius(int rowIndex)
        {
            return rowIndex switch
            {
                0 => new CornerRadius(50, 50, 0, 0),
                _ when rowIndex == MaxIndex => new CornerRadius(0, 0, 50, 50),
                _ => new CornerRadius(0, 0, 0, 0)
            };
        }
    }
}
