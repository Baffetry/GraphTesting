using StudentResultsSpace;

namespace Filters
{
    public interface ISorter
    {
        IEnumerable<StudentResults> Sort(ResultContainer container);
    }
}
