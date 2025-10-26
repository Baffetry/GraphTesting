using System.Text.Json.Serialization;

namespace TeacherGraphApplication.CWC
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
