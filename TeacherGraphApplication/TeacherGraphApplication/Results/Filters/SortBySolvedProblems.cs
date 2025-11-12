using StudentResultsSpace;

namespace Filters
{
    public class SortBySolvedProblems : Filter
    {
        public override IEnumerable<StudentResults> Sort(ResultContainer container)
        {
            var sorted = container.Students.OrderBy(s => s.SolvedProblems);
            return Ascending ? sorted : sorted.Reverse();
        }
    }
}
