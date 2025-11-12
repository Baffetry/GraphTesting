using StudentResultsSpace;

namespace Filters
{
    public class SortByRate : Filter
    {
        public override IEnumerable<StudentResults> Sort(ResultContainer container)
        {
            var sorted = container.Students.OrderBy(s => s.Rate);
            return Ascending ? sorted : sorted.Reverse();
        }
    }
}
