using StudentResultsSpace;

namespace Sorters
{
    public interface ISorter
    {
        IEnumerable<StudentResults> Sort(IEnumerable<StudentResults> students, bool ascending = true);
    }
}
