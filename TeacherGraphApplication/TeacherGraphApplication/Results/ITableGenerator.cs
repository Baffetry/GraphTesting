using Filters;

namespace Results
{
    public interface ITableGenerator
    {
        void DrawResults(string path, ISorter? sorter = null);
        void DrawLabels();
    }
}
