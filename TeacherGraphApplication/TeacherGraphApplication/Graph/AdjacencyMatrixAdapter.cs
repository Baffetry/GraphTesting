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
        private readonly Dictionary<string, int> _idToIndex; 
        private readonly bool[,] _matrix;

        public AdjacencyMatrixAdapter(GraphContainer graph)
        {
            _graph = graph ?? throw new ArgumentNullException(nameof(graph));
            _vertices = graph.Vertices.ToList();
            _idToIndex = new Dictionary<string, int>();

          
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
         

            foreach (var edge in _graph.Edges)
            {
                if (_idToIndex.TryGetValue(edge.Source.Id, out int i) &&
                    _idToIndex.TryGetValue(edge.Target.Id, out int j))
                {
                    _matrix[i, j] = true;
                    _matrix[j, i] = true; 
                }
            }
        }

       
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

       
        public bool[,] GetMatrix()
        {
            int n = _vertices.Count;
            var copy = new bool[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    copy[i, j] = _matrix[i, j];
            return copy;
        }

        public IReadOnlyList<GraphVertex> Vertices => _vertices;

        
        public int GetIndex(string vertexId) =>
            _idToIndex.TryGetValue(vertexId, out int index) ? index : -1;

        
        public int Size => _vertices.Count;

     
    }
}