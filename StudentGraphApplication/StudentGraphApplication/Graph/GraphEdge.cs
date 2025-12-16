using QuikGraph;
using System.Text.Json.Serialization;

namespace StudentGraphApplication.Graph
{
    public class GraphEdge
    {
        [JsonPropertyName("source")]
        public GraphVertex Source { get; set; }

        [JsonPropertyName("target")]
        public GraphVertex Target { get; set; }

        public GraphEdge(GraphVertex source, GraphVertex target)
        {
            Source = source;
            Target = target;
        }
    }
}
