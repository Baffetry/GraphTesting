using Generators;
using Filters;
using StudentResultsSpace;
using System.Windows;
using System.Windows.Controls;
using TeacherGraphApplication.Graph;

namespace TeacherGraphApplication.Results.Generators
{
    public class StudentResultGenerator
    {
        private Grid _grid;
        private ResultContainer container;
        private BorderGenerator borderGenerator;

        public StudentResultGenerator(Grid grid)
        {
            _grid = grid;

            container = new ResultContainer();

            borderGenerator = new BorderGenerator();
        }

        public void SetGraphContainer(GraphContainer graphContainer)
        {
            container.SetGraph(graphContainer);
        }

        public void GenerateResults(string path, ISorter? sorter = null)
        {
            _grid.Children.Clear();
            _grid.RowDefinitions.Clear();

            container.LoadFromFile(path);
            borderGenerator.MaxIndex = container.Students.Count - 1;

            var displayResults = sorter != null
                ? [.. sorter.Sort(container)]
                : container.Students;

            for (int i = 0; i < displayResults.Count; i++)
            {
                _grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(100, GridUnitType.Pixel) });

                List<Border> borders = SetBordersArray(i, displayResults).ToList();

                foreach (var border in borders)
                    Grid.SetRow(border, i);

                for (int index = 0; index < 4;  index++)
                    Grid.SetColumn(borders[index], index);

                foreach (var border in borders)
                    _grid.Children.Add(border);
            }
        }

        private IEnumerable<Border> SetBordersArray(int index, List<StudentResults> displayResults)
        {
            var result = displayResults[index];
            var student = result.Student;

            Border[] borders =
            {
                borderGenerator.GenerateBorder($"{student.LastName} {student.FirstName}", index),
                borderGenerator.GenerateBorder(result.SolvedProblems, index),
                borderGenerator.GenerateBorder(result.Percent, index),
                borderGenerator.GenerateBorder(result.Rate, index)
            };

            return borders;
        }
    }
}
