using StudentResultsSpace;

namespace Filters
{
    public class SortByPercent : Filter
    {
        public override IEnumerable<StudentResults> Sort(ResultContainer container)
        {
            var sorted = container.Students.OrderBy(x => x.Percent);
            return Ascending ? sorted : sorted.Reverse();
        }
    }
}
