namespace TeacherGraphApplication.Graph
{
    public class GraphVertex
    {
        public string Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public GraphVertex(string id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return Id;
        }
    }
}
