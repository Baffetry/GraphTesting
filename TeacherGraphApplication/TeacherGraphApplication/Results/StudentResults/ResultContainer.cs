using System.Collections;
using System.IO;
using System.Text.Json;

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
            if (students.Count > 0)
                students.Clear();

            if (File.Exists(path))
            {
                string[] studentsJson = File.ReadAllText(path).Split(new char[] { '\n', '\r' },
                    StringSplitOptions.RemoveEmptyEntries);
                var decrypter = new Encryption();

                foreach (var encodedJson in studentsJson)
                {
                    string json = decrypter.Decrypt(encodedJson);
                    var studentResult = JsonSerializer.Deserialize<StudentResults>(json);

                    studentResult.SolvedProblems = studentResult.TaskAnswers.Count;
                    studentResult.Percent = GetPersent(studentResult);
                    studentResult.Rate = GetRate(studentResult.Percent);

                    students.Add(studentResult);
                }
            }
        }

        private int GetRate(int persent)
        {
            return 1;
        }
        private int GetPersent(StudentResults result)
        {
            return 1;
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
