using System.Collections;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Windows;
using TeacherGraphApplication;
using TeacherGraphApplication.Graph;
using TeacherGraphApplication.Results.VariantManager;

namespace StudentResultsSpace
{
    public class ResultContainer : IEnumerable<StudentResults>
    {
        private List<StudentResults> students = new();
        Dictionary<string, object> Results;
        private GraphCalc graph;

        public ResultContainer() { }

        public int Count 
            => students.Count;

        public List<StudentResults> Students
            => students;

        public StudentResults this[int index]
            => students[index];

        public void SetGraph(GraphContainer container)
        {
            graph = new GraphCalc(container);

            Results = new Dictionary<string, object>()
            {
                {"Посчитайте цикломатическое число.", graph.GetCyclomaticNumber() },
                {"Посчитайте число независимости.", graph.GetIndependenceNumber() },
                {"Посчитайте хроматическое число.", graph.GetChromaticNumber() },
                {"Посчитайте радиус.", graph.GetRadius() },
                {"Посчитайте диаметр.", graph.GetDiameter() },
                {"Посчитайте число вершинного покрытия.", graph.GetVertexCoverNumber() },
                {"Посчитайте число реберного покрытия.", graph.GetEdgeCoverNumber() },
                {"Посчитайте плотность графа.", graph.GetDensity() },
                {"Посчитайте число паросочетания.", graph.GetMatchingNumber() },
                {"Посчитайте хроматический индекс.", graph.GetChromaticIndex() }
            };
        }

        public bool GraphInitialized()
        {
            return graph == null;
        }

        public void LoadFromFile(string path)
        {
            if (students.Count > 0)
                students.Clear();

            try
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

                        int totalQuestions = studentResult.TaskAnswers.Count;

                        int solvedQuestions = studentResult.TaskAnswers
                            .Where(x => x.Answer != null)
                            .Count();

                        int correctSolvedQuestions = studentResult.TaskAnswers
                            .Where(x => x.Answer.Equals(Results[x.Question]))
                            .Count();


                        studentResult.SolvedProblems = correctSolvedQuestions;
                        studentResult.Percent = totalQuestions > 0
                            ? (double)correctSolvedQuestions / totalQuestions
                            : 0;
                        studentResult.Rate = GetRate(studentResult.Percent);

                        students.Add(studentResult);
                    }
                }
            }
            catch (Exception)
            {
                throw new ArgumentException();
            }
        }

        private int GetRate(double percent)
        {
            return percent switch
            {
                >= 0.9 => 5,
                >= 0.75 => 4,
                >= 0.6 => 3,
                >= 0.4 => 2,
                _ => 1
            };
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
