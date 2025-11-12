using StudentResultsSpace;

namespace Filters
{
    public class SortByPercent(bool ascending) : ISorter
    {
        public IEnumerable<StudentResults> Sort(ResultContainer container)
        {
            var sorted = container.Students.OrderBy(x => x.Percent);
            return ascending ? sorted : sorted.Reverse();
        }
    }
}
