using System;
using System.Collections.Generic;
using System.Linq;
using TeacherGraphApplication.Graph;

namespace TeacherGraphApplication.Adapters
{
    public class AdjacencyMatrixAdapter
    {
        private readonly GraphContainer _graph;
        private readonly List<GraphVertex> _vertices;
        private readonly Dictionary<string, int> _idToIndex; // используем Id, т.к. GraphVertex не переопределяет Equals/GetHashCode
        private readonly bool[,] _matrix;

        public AdjacencyMatrixAdapter(GraphContainer graph)
        {
            _graph = graph ?? throw new ArgumentNullException(nameof(graph));
            _vertices = graph.Vertices.ToList();
            _idToIndex = new Dictionary<string, int>();

            // Индексируем вершины по Id (т.к. GraphVertex — reference type, и Id уникален)
            for (int i = 0; i < _vertices.Count; i++)
            {
                _idToIndex[_vertices[i].Id] = i;
            }

            int n = _vertices.Count;
            _matrix = new bool[n, n];

            BuildMatrix();
        }

        private void BuildMatrix()
        {
            int n = _vertices.Count;
            // Инициализируем диагональ — обычно false (если нет петель)
            // Но если у вас возможны петли — оставим как есть

            foreach (var edge in _graph.Edges)
            {
                if (_idToIndex.TryGetValue(edge.Source.Id, out int i) &&
                    _idToIndex.TryGetValue(edge.Target.Id, out int j))
                {
                    _matrix[i, j] = true;
                    _matrix[j, i] = true; // неориентированный граф
                }
            }
        }

        // Публичные методы

        /// <summary>
        /// Проверяет, соединены ли две вершины ребром.
        /// </summary>
        public bool AreAdjacent(GraphVertex a, GraphVertex b)
        {
            if (a == null || b == null) return false;
            return AreAdjacentById(a.Id, b.Id);
        }

        public bool AreAdjacentById(string idA, string idB)
        {
            if (!_idToIndex.TryGetValue(idA, out int i) ||
                !_idToIndex.TryGetValue(idB, out int j))
                return false;

            return _matrix[i, j];
        }

        /// <summary>
        /// Возвращает копию матрицы смежности.
        /// </summary>
        public bool[,] GetMatrix()
        {
            int n = _vertices.Count;
            var copy = new bool[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    copy[i, j] = _matrix[i, j];
            return copy;
        }

        /// <summary>
        /// Возвращает список вершин в порядке, соответствующем строкам/столбцам матрицы.
        /// </summary>
        public IReadOnlyList<GraphVertex> Vertices => _vertices;

        /// <summary>
        /// Возвращает индекс вершины по её Id.
        /// </summary>
        public int GetIndex(string vertexId) =>
            _idToIndex.TryGetValue(vertexId, out int index) ? index : -1;

        /// <summary>
        /// Возвращает размер матрицы (количество вершин).
        /// </summary>
        public int Size => _vertices.Count;

        /// <summary>
        /// Выводит матрицу в читаемом виде (для отладки).
        /// </summary>
        public override string ToString()
        {
            var lines = new List<string>();
            lines.Add("Adjacency Matrix:");

            // Заголовок
            lines.Add("     " + string.Join(" ", _vertices.Select(v => v.Id.PadLeft(3).Substring(0, Math.Min(3, v.Id.Length)))));

            for (int i = 0; i < _vertices.Count; i++)
            {
                var row = _vertices[i].Id.PadRight(4).Substring(0, Math.Min(4, _vertices[i].Id.Length)) + " ";
                for (int j = 0; j < _vertices.Count; j++)
                {
                    row += (_matrix[i, j] ? "1" : "0").PadLeft(3);
                }
                lines.Add(row);
            }

            return string.Join("\n", lines);
        }
    }
}