using System.Collections;
using System.IO;
using System.Text.Json;
using Sorters;

namespace StudentResultsSpace
{
    public class ResultContainer : IEnumerable<StudentResults>
    {
        private List<StudentResults> students = new();

        public int Count 
            => students.Count;

        public List<StudentResults> Students
            => students;

        public StudentResults this[int index]
            => students[index];

        public void LoadFromFile(string path)
        {
            if (File.Exists(path))
            {
                string[] studentsJson = File.ReadAllText(path).Split(new char[] { '\n', '\r' },
                    StringSplitOptions.RemoveEmptyEntries);
                var decrypter = new Encryption();

                foreach (var encodedJson in studentsJson)
                {
                    string json = decrypter.Decrypt(encodedJson);
                    var studentResult = JsonSerializer.Deserialize<StudentResults>(json);

                    studentResult.Percent = GetPersent(studentResult);
                    studentResult.Rate = GetRate(studentResult.Percent);

                    students.Add(studentResult);
                }
            }
        }

        private int GetRate(int persent)
        {
            //if (persent < 50) return 2;
            //else if (persent >= 50 && persent < 65) return 3;
            //else if (persent >= 65 && persent < 80) return 4;
            //else return 5;

            return persent / 100;
        }
        private int GetPersent(StudentResults result)
        {
            return result.TaskAnswers.Count;
        }

        #region IEnumerable
        public IEnumerator<StudentResults> GetEnumerator()
        {
            foreach (var student in students)
                yield return student;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
