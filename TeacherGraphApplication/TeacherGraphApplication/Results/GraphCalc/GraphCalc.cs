using TeacherGraphApplication.Graph;

namespace TeacherGraphApplication.Results.VariantManager
{
    public class GraphCalc
    {
        private GraphContainer _graph;

        public GraphCalc(GraphContainer graph) 
        {
            _graph = graph;
        }

        #region Основные свойства

        public int VertexCount => _graph.Vertices.Count;
        public int EdgeCount => _graph.Edges.Count;

        #endregion

        #region 1. Цикломатическое число

        public int GetCyclomaticNumber()
        {
            /* 
             * k = m - n + p;
             * m - число рёбер
             * n - число вершин
             * p - число компонент связности
             */

            int m = EdgeCount;
            int n = VertexCount;
            int p = GetConnectedComponents().Count;

            return m - n + p;
        }

        #endregion

        #region 2. Число независимости

        public int GetIndependenceNumber()
        {
            // Наибольшее множество попарно несмежных вершин
            // Используем приближенный алгоритм (жадный)
            var remainingVertices = new HashSet<GraphVertex>(_graph.Vertices);
            var independentSet = new List<GraphVertex>();

            while (remainingVertices.Count > 0)
            {
                // Выбираем вершину с минимальной степенью
                var vertex = remainingVertices
                    .OrderBy(v => GetDegree(v))
                    .First();

                independentSet.Add(vertex);

                // Удаляем выбранную вершину и всех ее соседей
                remainingVertices.Remove(vertex);
                foreach (var neighbor in _graph.GetNeighbors(vertex))
                {
                    remainingVertices.Remove(neighbor);
                }
            }

            return independentSet.Count;
        }
        #endregion

        #region 3. Хроматическое число

        public int GetChromaticNumber()
        {
            // Приближенный алгоритм последовательной раскраски
            if (VertexCount == 0) return 0;

            var colors = new Dictionary<GraphVertex, int>();
            var vertices = _graph.Vertices.ToList();

            // Сортируем вершины по убыванию степени (алгоритм Welsh-Powell)
            vertices = vertices.OrderByDescending(v => GetDegree(v)).ToList();

            int color = 0;
            while (colors.Count < vertices.Count)
            {
                color++;

                // Пытаемся покрасить в текущий цвет как можно больше вершин
                foreach (var vertex in vertices)
                {
                    if (colors.ContainsKey(vertex)) continue;

                    // Проверяем, нет ли соседей с таким же цветом
                    bool canColor = true;
                    foreach (var neighbor in _graph.GetNeighbors(vertex))
                    {
                        if (colors.ContainsKey(neighbor) && colors[neighbor] == color)
                        {
                            canColor = false;
                            break;
                        }
                    }

                    if (canColor)
                    {
                        colors[vertex] = color;
                    }
                }
            }

            return colors.Values.Max();
        }

        #endregion

        #region 4. Радиус и 5. Диаметр

        public int GetRadius()
        {
            var eccentricities = GetAllEccentricities();
            return eccentricities.Count > 0 ? eccentricities.Values.Min() : 0;
        }

        public int GetDiameter()
        {
            var eccentricities = GetAllEccentricities();
            return eccentricities.Count > 0 ? eccentricities.Values.Max() : 0;
        }

        private Dictionary<GraphVertex, int> GetAllEccentricities()
        {
            var eccentricities = new Dictionary<GraphVertex, int>();

            foreach (var vertex in _graph.Vertices)
            {
                eccentricities[vertex] = GetEccentricity(vertex);
            }

            return eccentricities;
        }

        private int GetEccentricity(GraphVertex vertex)
        {
            // Используем BFS для поиска наибольшего расстояния от вершины
            var distances = new Dictionary<GraphVertex, int>();
            var queue = new Queue<GraphVertex>();

            distances[vertex] = 0;
            queue.Enqueue(vertex);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                foreach (var neighbor in _graph.GetNeighbors(current))
                {
                    if (!distances.ContainsKey(neighbor))
                    {
                        distances[neighbor] = distances[current] + 1;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return distances.Values.Max();
        }

        #endregion

        #region 6. Число вершинного покрытия

        public int GetVertexCoverNumber()
        {
            // Теорема Кёнига: α(G) + β(G) = n
            // где α(G) - число независимости, β(G) - число вершинного покрытия
            int independenceNumber = GetIndependenceNumber();
            return VertexCount - independenceNumber;
        }

        #endregion

        #region 7. Число реберного покрытия

        public int GetEdgeCoverNumber()
        {
            // Для графа без изолированных вершин
            // ρ(G) = n - α'(G), где α'(G) - размер максимального паросочетания
            if (HasIsolatedVertices())
                return -1;

            int matchingNumber = GetMatchingNumber();
            return VertexCount - matchingNumber;
        }

        private bool HasIsolatedVertices()
        {
            return _graph.Vertices.Any(v => GetDegree(v) == 0);
        }
        #endregion

        #region 8. Плотность графа

        public double GetDensity()
        {
            // Для неориентированного графа: 2m / (n(n-1))
            int n = VertexCount;
            if (n <= 1) return 0;

            int m = EdgeCount;
            int maxPossibleEdges = n * (n - 1) / 2;

            return (double)m / maxPossibleEdges;
        }

        #endregion

        #region 9. Число паросочетания

        public int GetMatchingNumber()
        {
            // Приближенный алгоритм жадного паросочетания
            var matchedVertices = new HashSet<GraphVertex>();
            var matching = new List<GraphEdge>();
            var edges = _graph.Edges.ToList();

            // Сортируем ребра по некоторому критерию (например, по сумме степеней вершин)
            edges = edges.OrderByDescending(e => GetDegree(e.Source) + GetDegree(e.Target)).ToList();

            foreach (var edge in edges)
            {
                if (!matchedVertices.Contains(edge.Source) && !matchedVertices.Contains(edge.Target))
                {
                    matching.Add(edge);
                    matchedVertices.Add(edge.Source);
                    matchedVertices.Add(edge.Target);
                }
            }

            return matching.Count;
        }

        #endregion

        #region 10. Хроматический индекс

        public int GetChromaticIndex()
        {
            // Теорема Визинга: χ'(G) ≤ Δ(G) + 1
            // Для двудольных графов: χ'(G) = Δ(G)
            int maxDegree = GetMaximumDegree();

            if (IsBipartite())
            {
                return maxDegree;
            }
            else
            {
                // Для недвудольных графов может быть Δ(G) или Δ(G) + 1
                // Используем приближенный алгоритм
                return maxDegree + 1;
            }
        }

        private bool IsBipartite()
        {
            if (VertexCount == 0) return true;

            var colors = new Dictionary<GraphVertex, int>();
            var queue = new Queue<GraphVertex>();

            // Проверяем каждую компоненту связности
            foreach (var component in GetConnectedComponents())
            {
                if (component.Count == 0) continue;

                var startVertex = component.First();
                colors[startVertex] = 0;
                queue.Enqueue(startVertex);

                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();

                    foreach (var neighbor in _graph.GetNeighbors(current))
                    {
                        if (!colors.ContainsKey(neighbor))
                        {
                            colors[neighbor] = 1 - colors[current];
                            queue.Enqueue(neighbor);
                        }
                        else if (colors[neighbor] == colors[current])
                        {
                            return false; // Не двудольный
                        }
                    }
                }
            }

            return true;
        }

        #endregion

        #region Methods
        // Степень вершины
        private int GetDegree(GraphVertex vertex)
        {
            return _graph.GetEdges(vertex)
                .Count(e => e.Source == vertex || e.Target == vertex);
        }

        // Максимальная степень вершины в графе
        private int GetMaximumDegree()
        {
            if (_graph.Vertices.Count == 0) return 0;
            return _graph.Vertices.Max(v => GetDegree(v));
        }

        // Компоненты связности
        private List<List<GraphVertex>> GetConnectedComponents()
        {
            var visited = new HashSet<GraphVertex>();
            var components = new List<List<GraphVertex>>();

            foreach (var vertex in _graph.Vertices)
            {
                if (!visited.Contains(vertex))
                {
                    var component = new List<GraphVertex>();
                    BFS(vertex, visited, component);
                    components.Add(component);
                }
            }

            return components;
        }

        private void BFS(GraphVertex start, HashSet<GraphVertex> visited, List<GraphVertex> component)
        {
            var queue = new Queue<GraphVertex>();
            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                component.Add(current);

                // Получаем соседей через рёбра
                foreach (var edge in _graph.Edges)
                {
                    GraphVertex neighbor = null;
                    if (edge.Source == current) neighbor = edge.Target;
                    if (edge.Target == current) neighbor = edge.Source;

                    if (neighbor != null && !visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        #endregion
    }
}
