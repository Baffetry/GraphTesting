using Filters;
using Generators;
using System.Windows;
using TeacherGraphApplication;
using TeacherGraphApplication.Graph;
using TeacherGraphApplication.Results.Generators;

namespace Results
{
    public class TableGenerator : ITableGenerator
    {
        private static TableGenerator instance;

        private GridGenerator _gridGenerator;
        private static StudentResultGenerator _studentResultGenerator;

        private TableGenerator(GridGenerator gridGenerator, StudentResultGenerator resultGenerator)
        {
            _gridGenerator = gridGenerator;
            _studentResultGenerator = resultGenerator;
        }

        public static TableGenerator Instance(GridGenerator gridGenerator, StudentResultGenerator resultGenerator)
        {
            if (instance is null)
                instance = new TableGenerator(gridGenerator, resultGenerator);
            return instance;
        }

        public static void SetGraph(GraphContainer container, int taskCount)
        {
            _studentResultGenerator.SetGraphContainer(container, taskCount);
        }

        public static void GraphInitialized()
        {
            _studentResultGenerator.GraphInitialized();
        }

        public void DrawLabels()
        {
            _gridGenerator.SetLabelsInGrid();
        }

        public void DrawResults(string path, ISorter? sorter = null)
        {
            try
            {
                _studentResultGenerator.GenerateResults(path, sorter);
            }
            catch (Exception)
            {
                throw new ArgumentException("Выберите файл с результатами. . .");
            }
        }
    }
}
