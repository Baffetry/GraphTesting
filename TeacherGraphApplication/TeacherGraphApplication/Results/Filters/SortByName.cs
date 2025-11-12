using StudentResultsSpace;

namespace Filters
{
    public class SortByName : Filter
    {
        public override IEnumerable<StudentResults> Sort(ResultContainer container)
        {
            IEnumerable<StudentResults> sorted = container.Students
                    .OrderBy(s => s.Student.LastName)
                    .ThenBy(s => s.Student.FirstName);

            return Ascending ? sorted : sorted.Reverse();
        }
    }
}