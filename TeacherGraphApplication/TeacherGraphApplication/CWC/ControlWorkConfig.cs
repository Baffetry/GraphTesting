using System.Text.Json.Serialization;
using TeacherGraphApplication.Graph;

namespace TeacherGraphApplication.CWC
{

    public class ControlWorkConfig(int vertices, int edges, List<ControlWorkTask> taskList, GraphContainer graphContainer)
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

        [JsonPropertyName("Graph container")]
        [JsonInclude]
        public GraphContainer Container { get; set; } = graphContainer;
    }
}
