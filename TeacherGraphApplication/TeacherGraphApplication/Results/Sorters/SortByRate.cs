using StudentResultsSpace;

namespace Sorters
{
    public class SortByRate : ISorter
    {
        public IEnumerable<StudentResults> Sort(IEnumerable<StudentResults> students, bool ascending = true)
        {
            var sorted = students.OrderBy(s => s.Rate);
            return ascending ? sorted : sorted.Reverse();
        }
    }
}
