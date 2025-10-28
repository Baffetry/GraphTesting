using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudentGraphApplication
{
    public class ControlWorkConfig(int vertices, int edges, List<ControlWorkTask> taskList)
    {
        [JsonPropertyName("Amount of vertices")]
        [JsonInclude]
        public int Vertices { get; set; } = vertices;

        [JsonPropertyName("Amount of edges")]
        [JsonInclude]
        public int Edges { get; set; } = edges;

        [JsonPropertyName("Task list")]
        [JsonInclude]
        public List<ControlWorkTask>? TaskList { get; set; } = taskList;

    }
}
