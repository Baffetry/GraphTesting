using System.IO;
using System.Text.Json;

namespace Serializer
{
    public class Serializer<T>
    {
        public T LoadFromFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json);
        }

        public void SaveToFile(string path, T obj)
        {
            var json = JsonSerializer.Serialize(obj);
            File.WriteAllText(path, json);
        }
    }
}