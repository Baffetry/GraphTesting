using QuikGraph;
using System.Text.Json.Serialization;

namespace StudentGraphApplication.Graph
{
    public class GraphContainer
    {
        [JsonPropertyName("Vertices")]
        public List<GraphVertex> Vertices { get; set; } = new List<GraphVertex>();

        [JsonPropertyName("Edges")]
        public List<GraphEdge> Edges { get; set; } = new List<GraphEdge>();

        public GraphContainer()
        {
            Vertices = new List<GraphVertex>();
            Edges = new List<GraphEdge>();
        }

        public BidirectionalGraph<object, IEdge<object>> ToGraph()
        {
            var graph = new BidirectionalGraph<object, IEdge<object>>();

            // Добавляем все ID вершин
            foreach (var vertex in Vertices)
                graph.AddVertex(vertex.Id);

            // Добавляем ребра по ID
            foreach (var edge in Edges)
            {
                if (graph.ContainsVertex(edge.Source.Id) &&
                    graph.ContainsVertex(edge.Target.Id))
                {
                    graph.AddEdge(new Edge<string>(edge.Source.Id, edge.Target.Id));
                }
                else
                {
                    // Обработка отсутствующих вершин
                    if (!graph.ContainsVertex(edge.Source.Id))
                        graph.AddVertex(edge.Source.Id);
                    if (!graph.ContainsVertex(edge.Target.Id))
                        graph.AddVertex(edge.Target.Id);

                    graph.AddEdge(new Edge<string>(edge.Source.Id, edge.Target.Id));
                }
            }

            return graph;
        }
    }
}
