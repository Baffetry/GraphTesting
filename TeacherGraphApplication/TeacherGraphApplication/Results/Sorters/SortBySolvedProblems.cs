using StudentResultsSpace;

namespace Sorters
{
    public class SortBySolvedProblems : ISorter
    {
        public IEnumerable<StudentResults> Sort(IEnumerable<StudentResults> students, bool ascending = true)
        {
            var sorted = students.OrderBy(s => s.SolvedProblems);
            return ascending ? sorted : sorted.Reverse();
        }
    }
}
