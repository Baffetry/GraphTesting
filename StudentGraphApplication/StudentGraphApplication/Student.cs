using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudentGraphApplication
{
    public class Student
    {
        [JsonPropertyName("first_name")]
        [JsonInclude]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        [JsonInclude]
        public string LastName { get; set; }

        public Student(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

    }
}
