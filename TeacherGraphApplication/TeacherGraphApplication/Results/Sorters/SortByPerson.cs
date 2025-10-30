using StudentResultsSpace;

namespace Sorters
{
    public class SortByPerson : ISorter
    {
        public IEnumerable<StudentResults> Sort(IEnumerable<StudentResults> students, bool ascending = true)
        {
            var sorted = students.OrderBy(s => s.Student.LastName)
                           .ThenBy(s => s.Student.FirstName);

            return ascending ? sorted : sorted.Reverse();
        }
    }
}
