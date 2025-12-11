using System.Text.Json.Serialization;
using StudentGraphApplication.Graph;

namespace StudentGraphApplication
{
    public class ControlWorkConfig
    {
        [JsonPropertyName("Amount of vertices")]
        public int Vertices { get; set; }

        [JsonPropertyName("Amount of edges")]
        public int Edges { get; set; }

        [JsonPropertyName("Task list")]
        public List<ControlWorkTask>? TaskList { get; set; }

        [JsonPropertyName("Graph container")]
        public GraphContainer? Container { get; set; }

        public ControlWorkConfig() { }

        public ControlWorkConfig(int vertices, int edges, List<ControlWorkTask> taskList, GraphContainer graphContainer)
        {
            Vertices = vertices;
            Edges = edges;
            TaskList = taskList;
            Container = graphContainer;
        }
    }
}
