using System.Text.Json.Serialization;

namespace TeacherGraphApplication.CWC
{
    public class ControlWorkTask
    {
        [JsonPropertyName("Content")]
        [JsonInclude]
        public string Content;

        [JsonPropertyName("Answer")]
        [JsonInclude]
        public object Answer;

        public ControlWorkTask(string content, object answer)
        {
            Content = content;
            Answer = answer;
        }
    }
}
