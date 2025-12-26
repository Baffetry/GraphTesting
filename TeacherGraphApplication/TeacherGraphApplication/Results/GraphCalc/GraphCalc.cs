using System;
using System.Collections.Generic;
using System.Linq;
using TeacherGraphApplication.Adapters;
using TeacherGraphApplication.Graph;
//using TeacherGraphApplication.Results.GraphCalc;

namespace TeacherGraphApplication.Results.VariantManager
{
    public class GraphCalc
    {
        private GraphContainer _graph;
        private AdjacencyMatrixAdapter _matrixAdapter;

        public GraphCalc(GraphContainer graph)
        {
            _graph = graph ?? throw new ArgumentNullException(nameof(graph));
            _matrixAdapter = new AdjacencyMatrixAdapter(graph);
        }

        #region Основные свойства

        public int VertexCount => _graph.Vertices.Count;
        public int EdgeCount => _graph.Edges.Count;

        #endregion

        #region 1. Цикломатическое число — РЕАЛИЗОВАНО

        public int GetCyclomaticNumber()
        {
            int m = EdgeCount;
            int n = VertexCount;

            return m - n + 1;
        }

        #endregion

        #region 2. Число независимости — РЕАЛИЗОВАНО

        public int GetIndependenceNumber()
        {
            int n = VertexCount;
            if (n == 0) return 0;

            bool[,] matrix = _matrixAdapter.GetMatrix();

            var independentSet = new HashSet<int>();
            var available = new HashSet<int>(Enumerable.Range(0, n));

            while (available.Count > 0)
            {
                int minDegreeVertex = -1;
                int minDegree = int.MaxValue;

                foreach (int v in available)
                {
                    int degree = 0;
                    foreach (int u in available)
                    {
                        if (matrix[v, u]) degree++;
                    }

                    if (degree < minDegree)
                    {
                        minDegree = degree;
                        minDegreeVertex = v;
                    }
                }

                if (minDegreeVertex == -1) break;

                independentSet.Add(minDegreeVertex);
                available.Remove(minDegreeVertex);
                for (int u = 0; u < n; u++)
                {
                    if (matrix[minDegreeVertex, u])
                    {
                        available.Remove(u);
                    }
                }
            }

            return independentSet.Count;
        }

        #endregion

        #region 3. Хроматическое число — РЕАЛИЗОВАНО

        public int GetChromaticNumber()
        {
            int n = VertexCount;
            if (n == 0) return 0;

            bool[,] matrix = _matrixAdapter.GetMatrix();
            int[] colors = new int[n];

          
            for (int k = 1; k <= n; k++)
            {
                Array.Fill(colors, -1);
                if (TryColorGraph(matrix, colors, 0, k))
                {
                    return k;
                }
            }

            return n; 
        }
        private bool TryColorGraph(bool[,] matrix, int[] colors, int vertex, int k)
        {
            int n = colors.Length;
            if (vertex == n)
            {
                return true;
            }

            for (int c = 0; c < k; c++)
            {
                if (IsColorSafe(matrix, colors, vertex, c))
                {
                   
                    colors[vertex] = c;

                   
                    if (TryColorGraph(matrix, colors, vertex + 1, k))
                    {
                        return true;
                    }

                    colors[vertex] = -1;
                }
            }

            return false;
        }

        private bool IsColorSafe(bool[,] matrix, int[] colors, int v, int c)
        {
            int n = colors.Length;

            for (int u = 0; u < n; u++)
            {
                if (matrix[v, u] && colors[u] == c)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region 4. Радиус и 5. Диаметр — РЕАЛИЗОВАНО

        public int GetRadius()
        {
            int n = VertexCount;
            if (n == 0) return 0;

            int[,] dist = GetAllPairsDistances();
            int minEccentricity = int.MaxValue;

            for (int i = 0; i < n; i++)
            {
                int maxDist = 0;
                for (int j = 0; j < n; j++)
                {
                    if (i != j) maxDist = Math.Max(maxDist, dist[i, j]);
                }
                minEccentricity = Math.Min(minEccentricity, maxDist);
            }

            return minEccentricity;
        }

        public int GetDiameter()
        {
            int n = VertexCount;
            if (n == 0) return 0;

            int[,] dist = GetAllPairsDistances();
            int maxDistance = 0;

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    maxDistance = Math.Max(maxDistance, dist[i, j]);
                }
            }

            return maxDistance;
        }

        private int[,] GetAllPairsDistances()
        {
            int n = VertexCount;
            int[,] dist = new int[n, n];
            bool[,] matrix = _matrixAdapter.GetMatrix();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                        dist[i, j] = 0;
                    else if (matrix[i, j])
                        dist[i, j] = 1;
                    else
                        dist[i, j] = int.MaxValue / 2;
                }
            }

            for (int k = 0; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    if (dist[i, k] == int.MaxValue / 2) continue;
                    for (int j = 0; j < n; j++)
                    {
                        if (dist[k, j] == int.MaxValue / 2) continue;
                        if (dist[i, k] + dist[k, j] < dist[i, j])
                        {
                            dist[i, j] = dist[i, k] + dist[k, j];
                        }
                    }
                }
            }

            return dist;
        }

        #endregion

        #region 6. Число вершинного покрытия — РЕАЛИЗОВАНО

        public int GetVertexCoverNumber()
        {
            return VertexCount - GetIndependenceNumber();
        }

        #endregion

        #region 7. Число реберного покрытия — РЕАЛИЗОВАНО

        public int GetEdgeCoverNumber()
        {
            int n = VertexCount;
            if (n == 0) return 0;

            bool[,] matrix = _matrixAdapter.GetMatrix();


            int minEdges = (int)Math.Ceiling(n / 2.0);
            int maxEdges = n - 1;


            for (int i = 0; i < n; i++)
            {
                bool hasNeighbor = false;
                for (int j = 0; j < n; j++)
                {
                    if (matrix[i, j] && i != j)
                    {
                        hasNeighbor = true;
                        break;
                    }
                }
                if (!hasNeighbor)
                {
                    return -1;
                }
            }

            for (int k = minEdges; k <= maxEdges; k++)
            {
                if (TryFindEdgeCover(matrix, n, k))
                {
                    return k;
                }
            }

            return maxEdges;
        }

        private bool TryFindEdgeCover(bool[,] matrix, int n, int k)
        {
            var allEdges = GetAllEdges(matrix, n);

            if (allEdges.Count < k) return false;
            var combinations = GenerateCombinations(allEdges, k);

            foreach (var edgeSet in combinations)
            {
                if (IsEdgeCover(edgeSet, n))
                {
                    return true;
                }
            }

            return false;
        }

        private List<(int, int)> GetAllEdges(bool[,] matrix, int n)
        {
            var edges = new List<(int, int)>();

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (matrix[i, j])
                    {
                        edges.Add((i, j));
                    }
                }
            }

            return edges;
        }

        private IEnumerable<List<(int, int)>> GenerateCombinations(List<(int, int)> edges, int k)
        {
            var result = new List<List<(int, int)>>();
            GenerateCombinationsRecursive(edges, k, 0, new List<(int, int)>(), result);
            return result;
        }

        private void GenerateCombinationsRecursive(List<(int, int)> edges, int k, int start,
                                                   List<(int, int)> current, List<List<(int, int)>> result)
        {
            if (current.Count == k)
            {
                result.Add(new List<(int, int)>(current));
                return;
            }

            for (int i = start; i < edges.Count; i++)
            {
                current.Add(edges[i]);
                GenerateCombinationsRecursive(edges, k, i + 1, current, result);
                current.RemoveAt(current.Count - 1);
            }
        }

        private bool IsEdgeCover(List<(int, int)> edgeSet, int n)
        {
            bool[] covered = new bool[n];

            foreach (var (u, v) in edgeSet)
            {
                covered[u] = true;
                covered[v] = true;
            }
            for (int i = 0; i < n; i++)
            {
                if (!covered[i])
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region 8. Плотность графа — ИСПРАВЛЕНО

        public double GetDensity()
        {
            int n = VertexCount;
            if (n == 0) return 0;

            bool[,] matrix = _matrixAdapter.GetMatrix();
            int[] colors = new int[n];
            List<int> vertices = new List<int>();

            for (int i = 0; i < n; i++)
            {
                vertices.Add(i);
            }

            vertices.Sort((a, b) =>
            {
                int degA = 0, degB = 0;
                for (int i = 0; i < n; i++)
                {
                    if (matrix[a, i]) degA++;
                    if (matrix[b, i]) degB++;
                }
                return degB.CompareTo(degA);
            });

            int maxClique = 0;
            List<int> current = new List<int>();

            BronKerbosch(matrix, vertices, current, ref maxClique, 0, n);

            return maxClique;
        }

        private void BronKerbosch(bool[,] matrix, List<int> candidates,
                                  List<int> current, ref int maxClique,
                                  int depth, int n)
        {
            if (candidates.Count == 0)
            {
                if (current.Count > maxClique)
                {
                    maxClique = current.Count;
                }
                return;
            }

            if (current.Count + candidates.Count <= maxClique)
            {
                return;
            }

            int pivot = candidates[0];
            List<int> candidatesWithoutPivot = new List<int>();

            foreach (int v in candidates)
            {
                if (v != pivot && matrix[v, pivot])
                {
                    candidatesWithoutPivot.Add(v);
                }
            }

            while (candidates.Count > 0)
            {
                int v = candidates[0];
                current.Add(v);


                List<int> newCandidates = new List<int>();
                foreach (int u in candidates)
                {
                    if (matrix[v, u])
                    {
                        newCandidates.Add(u);
                    }
                }

                BronKerbosch(matrix, newCandidates, current, ref maxClique, depth + 1, n);


                current.RemoveAt(current.Count - 1);
                candidates.Remove(v);


                if (current.Count + candidates.Count <= maxClique)
                {
                    break;
                }
            }
        }

        #endregion

        #region 9. Число паросочетания — РЕАЛИЗОВАНО

        public int GetMatchingNumber()
        {
            int n = VertexCount;
            if (n == 0) return 0;

            int edgeCoverNumber = GetEdgeCoverNumber();

            if (edgeCoverNumber == -1)
            {

                return FindMatchingDirectly();
            }

            return n - edgeCoverNumber;
        }

        private int FindMatchingDirectly()
        {
            int n = VertexCount;
            if (n == 0) return 0;

            bool[,] matrix = _matrixAdapter.GetMatrix();
            bool[] used = new bool[n];
            int matching = 0;

            for (int i = 0; i < n; i++)
            {
                if (!used[i])
                {
                    for (int j = i + 1; j < n; j++)
                    {
                        if (!used[j] && matrix[i, j])
                        {
                            used[i] = true;
                            used[j] = true;
                            matching++;
                            break;
                        }
                    }
                }
            }

            return matching;
        }

        #endregion

        #region 10. Хроматический индекс — РЕАЛИЗОВАНО

        public int GetChromaticIndex()
        {
            int n = VertexCount;
            if (n == 0 || EdgeCount == 0) return 0;

            bool[,] matrix = _matrixAdapter.GetMatrix();
            int maxDegree = GetMaxDegree(matrix);
            if (maxDegree <= 2)
            {
                if (TryEdgeColoring(matrix, maxDegree))
                    return maxDegree;
                else
                    return maxDegree + 1;
            }
            for (int colors = maxDegree; colors <= maxDegree + 1; colors++)
            {
                if (TryEdgeColoring(matrix, colors))
                    return colors;
            }

            return maxDegree + 1;
        }

        private int GetMaxDegree(bool[,] matrix)
        {
            int n = VertexCount;
            int maxDegree = 0;

            for (int i = 0; i < n; i++)
            {
                int degree = 0;
                for (int j = 0; j < n; j++)
                {
                    if (matrix[i, j]) degree++;
                }
                maxDegree = Math.Max(maxDegree, degree);
            }

            return maxDegree;
        }

        private bool TryEdgeColoring(bool[,] matrix, int numColors)
        {
            int n = VertexCount;

            List<Tuple<int, int>> edges = new List<Tuple<int, int>>();
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (matrix[i, j])
                    {
                        edges.Add(Tuple.Create(i, j));
                    }
                }
            }

            if (edges.Count == 0) return true;
            edges = edges.OrderByDescending(e =>
            {
                int v1 = e.Item1, v2 = e.Item2;
                int deg1 = 0, deg2 = 0;
                for (int k = 0; k < n; k++)
                {
                    if (matrix[v1, k]) deg1++;
                    if (matrix[v2, k]) deg2++;
                }
                return deg1 + deg2;
            }).ToList();
            int[] edgeColors = new int[edges.Count];
            for (int i = 0; i < edgeColors.Length; i++) edgeColors[i] = -1;
            List<HashSet<int>> vertexColorSets = new List<HashSet<int>>();
            for (int i = 0; i < n; i++)
            {
                vertexColorSets.Add(new HashSet<int>());
            }
            return ColorEdgesBacktracking(0, edges, edgeColors, vertexColorSets, matrix, numColors);
        }

        private bool ColorEdgesBacktracking(int edgeIndex, List<Tuple<int, int>> edges,
            int[] edgeColors, List<HashSet<int>> vertexColorSets, bool[,] matrix, int numColors)
        {
            if (edgeIndex >= edges.Count)
                return true;

            var edge = edges[edgeIndex];
            int u = edge.Item1;
            int v = edge.Item2;
            for (int color = 0; color < numColors; color++)
            {
                if (!vertexColorSets[u].Contains(color) &&
                    !vertexColorSets[v].Contains(color))
                {
                    edgeColors[edgeIndex] = color;
                    vertexColorSets[u].Add(color);
                    vertexColorSets[v].Add(color);

                    if (ColorEdgesBacktracking(edgeIndex + 1, edges, edgeColors, vertexColorSets, matrix, numColors))
                        return true;
                    vertexColorSets[u].Remove(color);
                    vertexColorSets[v].Remove(color);
                    edgeColors[edgeIndex] = -1;
                }
            }

            return false;
        }

        #endregion

        #region Вспомогательные методы

        public string PrintMatrix()
        {
            return _matrixAdapter.ToString();
        }
        public string GetDensityString()
        {
            double density = GetDensity();
            return density.ToString("0.##").Replace(',', '.');
        }

        #endregion
    }
}