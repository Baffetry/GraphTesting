using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherGraphApplication.Graph
{
    public class GraphEdge
    {
        // Дополнительный класс для хранения ребра

        public GraphVertex Source { get; set; }
        public GraphVertex Target { get; set; }

        public GraphEdge(GraphVertex source, GraphVertex target)
        {
            Source = source;
            Target = target;
        }

    }
}
