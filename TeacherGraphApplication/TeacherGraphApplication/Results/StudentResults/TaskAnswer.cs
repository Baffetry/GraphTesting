using System.Text.Json.Serialization;

namespace StudentResultsSpace
{
    public class TaskAnswer
    {
        [JsonPropertyName("question")]
        [JsonInclude]
        public string Question { get; set; }

        [JsonPropertyName("answer")]
        [JsonInclude]
        public object Answer { get; set; }

        public TaskAnswer() { }

        public TaskAnswer(string question, object answer)
        {
            Question = question;
            Answer = answer;
        }

        public override string ToString()
        {
            return $"Вопрос: {Question}\nОтвет: {Answer}";
        }
    }
}
