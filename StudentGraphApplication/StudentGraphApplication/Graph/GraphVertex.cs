using System.Text.Json.Serialization;

namespace StudentGraphApplication.Graph
{
    public class GraphVertex
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("x")]
        public double X { get; set; }

        [JsonPropertyName("y")]
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
