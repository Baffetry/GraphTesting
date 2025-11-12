using StudentResultsSpace;

namespace Filters
{
    public class SortByName(bool ascending) : ISorter
    {
        public IEnumerable<StudentResults> Sort(ResultContainer container)
        {
            IEnumerable<StudentResults> sorted = container.Students
                    .OrderBy(s => s.Student.LastName)
                    .ThenBy(s => s.Student.FirstName);

            return ascending ? sorted : sorted.Reverse();
        }
    }
}