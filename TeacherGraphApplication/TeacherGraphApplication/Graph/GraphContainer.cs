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

        [JsonIgnore]
        private Dictionary<GraphVertex, List<GraphEdge>> adjacencyList = new Dictionary<GraphVertex, List<GraphEdge>>();

        // Метод для добавления вершины
        public void AddVertex(GraphVertex vertex)
        {
            if (!Vertices.Contains(vertex))
            {
                Vertices.Add(vertex);
                adjacencyList[vertex] = new List<GraphEdge>();
            }
        }

        // Метод для добавления ребра
        public void AddEdge(GraphEdge edge)
        {
            if (!Edges.Contains(edge))
            {
                Edges.Add(edge);

                // Добавляем в списки смежности для быстрого доступа
                if (!adjacencyList.ContainsKey(edge.Source))
                    adjacencyList[edge.Source] = new List<GraphEdge>();
                if (!adjacencyList.ContainsKey(edge.Target))
                    adjacencyList[edge.Target] = new List<GraphEdge>();

                adjacencyList[edge.Source].Add(edge);
                adjacencyList[edge.Target].Add(edge);
            }
        }

        // Метод для создания ребра между двумя вершинами
        public GraphEdge AddEdge(GraphVertex source, GraphVertex target)
        {
            var edge = new GraphEdge(source, target);
            AddEdge(edge);
            return edge;
        }

        // Получить все инцидентные ребра для вершины
        public List<GraphEdge> GetEdges(GraphVertex vertex)
        {
            return adjacencyList.ContainsKey(vertex)
                ? new List<GraphEdge>(adjacencyList[vertex])
                : new List<GraphEdge>();
        }

        // Получить соседние вершины для вершины
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

        // Проверить, существует ли ребро между вершинами
        public bool HasEdge(GraphVertex source, GraphVertex target)
        {
            if (!adjacencyList.ContainsKey(source))
                return false;

            return adjacencyList[source].Any(edge =>
                (edge.Source == source && edge.Target == target) ||
                (edge.Source == target && edge.Target == source));
        }

        // Очистить граф
        public void Clear()
        {
            Vertices.Clear();
            Edges.Clear();
            adjacencyList.Clear();
        }

        public BidirectionalGraph<object, IEdge<object>> ToGraphLayoutGraph()
        {
            var graph = new BidirectionalGraph<object, IEdge<object>>();

            // Добавляем вершины
            foreach (var vertex in Vertices)
                graph.AddVertex(vertex);

            // Добавляем ребра
            foreach (var edge in Edges)
                graph.AddEdge(new Edge<object>(edge.Source, edge.Target));

            return graph;
        }
    }
}
