using TeacherGraphApplication.Results.Generators;
using Generators;
using Filters;

namespace Results
{
    public class TableGenerator : ITableGenerator
    {
        private static TableGenerator instance;

        private GridGenerator _gridGenerator;
        private StudentResultGenerator _studentResultGenerator;

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

        public void DrawLabels()
        {
            _gridGenerator.SetLabelsInGrid();
        }

        public void DrawResults(string path, ISorter? sorter = null)
        {
            _studentResultGenerator.GenerateResults(path, sorter);
        }
    }
}
