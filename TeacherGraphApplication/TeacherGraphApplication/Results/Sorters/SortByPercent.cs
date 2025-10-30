using StudentResultsSpace;

namespace Sorters
{
    public class SortByPercent : ISorter
    {
        public IEnumerable<StudentResults> Sort(IEnumerable<StudentResults> students, bool ascending = true)
        {
            var sorted = students.OrderBy(x => x.Percent);
            return ascending ? sorted : sorted.Reverse();
        }
    }
}
