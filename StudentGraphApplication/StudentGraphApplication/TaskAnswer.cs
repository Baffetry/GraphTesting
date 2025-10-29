using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudentGraphApplication
{
    public class TaskAnswer
    {
        [JsonPropertyName("question")]
        [JsonInclude]
        public string Question { get; set; }

        [JsonPropertyName("answer")]
        [JsonInclude]
        public object Answer { get; set; }

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
