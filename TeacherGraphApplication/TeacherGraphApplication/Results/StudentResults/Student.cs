using System.Text.Json.Serialization;

namespace StudentResultsSpace
{
    public class Student
    {
        [JsonPropertyName("first_name")]
        [JsonInclude]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        [JsonInclude]
        public string LastName { get; set; }

        public Student() { }

        public Student(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

    }
}