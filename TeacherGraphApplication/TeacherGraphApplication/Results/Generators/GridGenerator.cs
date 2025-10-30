using System.Windows.Controls;
using System.Windows;

namespace Generators
{
    public class GridGenerator(Grid grid)
    {
        private Grid _grid = grid;

        private string[] _labels =
        {
            "Студент",
            "Решено",
            "Процент",
            "Оценка"
        };

        public void SetLabelsInGrid()
        {
            for (int i = 0; i < _labels.Length; i++)
            {
                _grid.ColumnDefinitions.Add(new ColumnDefinition 
                { 
                    Width = new GridLength(437, GridUnitType.Pixel)
                });

                var label = new LabelGenerator().GenerateLabel(_labels[i]);

                Grid.SetColumn(label, i);
                _grid.Children.Add(label);
            }
        }
    }
}
