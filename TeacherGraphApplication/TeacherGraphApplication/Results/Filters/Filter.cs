using StudentResultsSpace;

namespace Filters
{
    public abstract class Filter : ISorter
    {
        public bool Ascending { get; set; }

        abstract public IEnumerable<StudentResults> Sort(ResultContainer container);
    }
}
