using QuikGraph;
using System.Text.Json.Serialization;

namespace TeacherGraphApplication.Graph
{
    public class GraphContainer
    {
        [JsonPropertyName("Vertices")]
        [JsonInclude]
        public List<GraphVertex> Vertices { get; set; } = new List<GraphVertex>();

        [JsonPropertyName("Edges")]
        [JsonInclude]
        public List<GraphEdge> Edges { get; set; } = new List<GraphEdge>();

        private Dictionary<GraphVertex, List<GraphEdge>> adjacencyList = new Dictionary<GraphVertex, List<GraphEdge>>();
        public void AddVertex(GraphVertex vertex)
        {
            if (!Vertices.Contains(vertex))
            {
                Vertices.Add(vertex);
                adjacencyList[vertex] = new List<GraphEdge>();
            }
        }
        public void AddEdge(GraphEdge edge)
        {
            if (!Edges.Contains(edge))
            {
                Edges.Add(edge);
                if (!adjacencyList.ContainsKey(edge.Source))
                    adjacencyList[edge.Source] = new List<GraphEdge>();
                if (!adjacencyList.ContainsKey(edge.Target))
                    adjacencyList[edge.Target] = new List<GraphEdge>();

                adjacencyList[edge.Source].Add(edge);
                adjacencyList[edge.Target].Add(edge);
            }
        }

        public GraphEdge AddEdge(GraphVertex source, GraphVertex target)
        {
            var edge = new GraphEdge(source, target);
            AddEdge(edge);
            return edge;
        }

        public List<GraphEdge> GetEdges(GraphVertex vertex)
        {
            return adjacencyList.ContainsKey(vertex)
                ? new List<GraphEdge>(adjacencyList[vertex])
                : new List<GraphEdge>();
        }

        public List<GraphVertex> GetNeighbors(GraphVertex vertex)
        {
            var neighbors = new HashSet<GraphVertex>();

            if (adjacencyList.ContainsKey(vertex))
            {
                foreach (var edge in adjacencyList[vertex])
                {
                    if (edge.Source != vertex) neighbors.Add(edge.Source);
                    if (edge.Target != vertex) neighbors.Add(edge.Target);
                }
            }

            return neighbors.ToList();
        }

        public bool HasEdge(GraphVertex source, GraphVertex target)
        {
            if (!adjacencyList.ContainsKey(source))
                return false;

            return adjacencyList[source].Any(edge =>
                (edge.Source == source && edge.Target == target) ||
                (edge.Source == target && edge.Target == source));
        }

        public void Clear()
        {
            Vertices.Clear();
            Edges.Clear();
            adjacencyList.Clear();
        }

        public BidirectionalGraph<object, IEdge<object>> ToGraphLayoutGraph()
        {
            var graph = new BidirectionalGraph<object, IEdge<object>>();

          
            foreach (var vertex in Vertices)
                graph.AddVertex(vertex);

        
            foreach (var edge in Edges)
                graph.AddEdge(new Edge<object>(edge.Source, edge.Target));

            return graph;
        }
    }
}
