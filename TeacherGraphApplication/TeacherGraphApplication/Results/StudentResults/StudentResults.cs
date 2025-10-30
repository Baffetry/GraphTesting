﻿using System.Text.Json.Serialization;

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

        public int Percent { get; set; }

        public int Rate { get; set; }

        public int SolvedProblems { get; set; }

        public StudentResults() { }
    }
}
