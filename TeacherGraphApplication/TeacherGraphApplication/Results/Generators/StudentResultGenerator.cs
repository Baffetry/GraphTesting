using Generators;
using Sorters;
using StudentResultsSpace;
using System.Windows;
using System.Windows.Controls;

namespace TeacherGraphApplication.Results.Generators
{
    public class StudentResultGenerator
    {
        private Grid _grid;
        private ResultContainer container;
        private BorderGenerator borderGenerator;
        private List<StudentResults> results;

        public StudentResultGenerator(Grid grid)
        {
            _grid = grid;

            container = new ResultContainer();
            results = container.Students;

            borderGenerator = new BorderGenerator();
        }

        public void GenerateResults(string path, ISorter sorter = null)
        {
            container.LoadFromFile(path);
            borderGenerator.MaxIndex = results.Count - 1;

            if (sorter != null)
                results = (List<StudentResults>)sorter.Sort(container);

            for (int i = 0; i < results.Count; i++)
            {
                _grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(100, GridUnitType.Pixel) });

                List<Border> borders = SetBordersArray(i).ToList();

                foreach (var border in borders)
                    Grid.SetRow(border, i);

                for (int index = 0; index < 4;  index++)
                    Grid.SetColumn(borders[index], index);

                foreach (var border in borders)
                    _grid.Children.Add(border);
            }
        }

        private IEnumerable<Border> SetBordersArray(int index)
        {
            var result = container[index];
            var student = result.Student;

            Border[] borders =
            {
                borderGenerator.GenerateBorder($"{student.FirstName} {student.LastName}", index),
                borderGenerator.GenerateBorder(result.TaskAnswers.Count, index),
                borderGenerator.GenerateBorder(result.Percent, index),
                borderGenerator.GenerateBorder(result.Rate, index)
            };

            return borders;
        }
    }
}
