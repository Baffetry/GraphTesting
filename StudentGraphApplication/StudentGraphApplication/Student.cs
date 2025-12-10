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
        public string FirstName { get; private set; } = string.Empty;

        [JsonPropertyName("last_name")]
        [JsonInclude]
        public string LastName { get; private set; } = string.Empty;

        [JsonIgnore]
        public bool IsInitialized => !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName);

        private static Student _instance;

        private static readonly object _lock = new object();

        private Student()
        {

        }

        public static Student GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Student();
                    }
                }
            }
            return _instance;
        }

        public void Initialize(string lastName, string firstName)
        {
            if (IsInitialized)
            {
                throw new InvalidOperationException("Данные студента уже инициализированы");
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("Фамилия не может быть пустой", nameof(lastName));
            }

            LastName = lastName.Trim();
            FirstName = firstName?.Trim() ?? string.Empty;
        }

        public static void ResetInstance()
        {
            lock (_lock)
            {
                _instance = null;
            }
        }

        public override string ToString()
        {
            if (!IsInitialized)
                return "Студент не инициализирован";

            return $"{LastName} {FirstName}".Trim();
        }
    }
}