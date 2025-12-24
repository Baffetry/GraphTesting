using System.Text.Json.Serialization;
using System.Windows.Navigation;

namespace StudentResultsSpace
{
    public class StudentResults
    {
        [JsonPropertyName("student")]
        [JsonInclude]
        public Student Student { get; set; }

        [JsonPropertyName("task_answers")]
        [JsonInclude]
        public List<TaskAnswer> TaskAnswers { get; set; }

        public double Percent { get; set; }

        public int Rate { get; set; }

        public int SolvedProblems { get; set; }

        public int TotalTask { get; set; }

        public StudentResults() { }

        public string GetSP()
            => $"{SolvedProblems} / {TotalTask}";

        public string GetPercent()
            => $"{Percent}%";
    }
}
