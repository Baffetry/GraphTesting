using StudentResultsSpace;

namespace Filters
{
    public class SortByRate(bool ascending) : ISorter
    {
        public IEnumerable<StudentResults> Sort(ResultContainer container)
        {
            var sorted = container.Students.OrderBy(s => s.Rate);
            return ascending ? sorted : sorted.Reverse();
        }
    }
}
