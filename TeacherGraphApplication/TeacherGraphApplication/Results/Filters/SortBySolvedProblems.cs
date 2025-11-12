using StudentResultsSpace;

namespace Filters
{
    public class SortBySolvedProblems(bool ascending) : ISorter
    {
        public IEnumerable<StudentResults> Sort(ResultContainer container)
        {
            var sorted = container.Students.OrderBy(s => s.SolvedProblems);
            return ascending ? sorted : sorted.Reverse();
        }
    }
}
