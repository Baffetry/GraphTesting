using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudentGraphApplication
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
