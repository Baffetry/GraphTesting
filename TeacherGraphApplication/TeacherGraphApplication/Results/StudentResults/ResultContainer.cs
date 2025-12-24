using System.Collections;
using System.Globalization;
using System.IO;
using System.Text.Json;
using TeacherGraphApplication.Graph;
using TeacherGraphApplication.Results.VariantManager;

namespace StudentResultsSpace
{
    public class ResultContainer : IEnumerable<StudentResults>
    {
        private List<StudentResults> students = new();
        private Dictionary<string, object> results;
        private GraphCalc graph;

        private int TaskCount;

        public ResultContainer() { }

        public int Count
            => students.Count;

        public List<StudentResults> Students
            => students;

        public StudentResults this[int index]
            => students[index];

        public void SetGraph(GraphContainer container, int taskCount)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            TaskCount = taskCount;

            graph = new GraphCalc(container);
            results = new Dictionary<string, object>()
            {
                {"Посчитайте цикломатическое число.", graph.GetCyclomaticNumber() },
                {"Посчитайте число независимости.", graph.GetIndependenceNumber() },
                {"Посчитайте хроматическое число.", graph.GetChromaticNumber() },
                {"Посчитайте радиус.", graph.GetRadius() > 1000 ? -1 : graph.GetRadius()},
                {"Посчитайте диаметр.", graph.GetDiameter() > 1000 ? -1 : graph.GetDiameter() },
                {"Посчитайте число вершинного покрытия.", graph.GetVertexCoverNumber() },
                {"Посчитайте число реберного покрытия.", graph.GetEdgeCoverNumber() },
                {"Посчитайте плотность графа.", graph.GetDensity() },
                {"Посчитайте число паросочетания.", graph.GetMatchingNumber() },
                {"Посчитайте хроматический индекс.", graph.GetChromaticIndex() }
            };
        }

        public bool GraphInitialized()
        {
            return graph != null && results != null;
        }

        public void LoadFromFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or empty", nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}");
            students.Clear();

            try
            {
                string fileContent = File.ReadAllText(path);
                string[] studentsJson = fileContent.Split(new char[] { '\n', '\r' },
                    StringSplitOptions.RemoveEmptyEntries);

                var decrypter = new Encryption();

                foreach (var encodedJson in studentsJson)
                {
                    if (string.IsNullOrWhiteSpace(encodedJson))
                        continue;

                    string json = decrypter.Decrypt(encodedJson);
                    var studentResult = JsonSerializer.Deserialize<StudentResults>(json);

                    if (studentResult == null || studentResult.TaskAnswers == null)
                        continue;

                    CalculateStudentResults(studentResult);
                    students.Add(studentResult);
                }
            }
            catch (JsonException ex)
            {
                throw new ArgumentException($"Invalid JSON format in file: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error loading file: {ex.Message}", ex);
            }
        }

        private void CalculateStudentResults(StudentResults studentResult)
        {
            if (studentResult == null || studentResult.TaskAnswers == null || results == null)
            {
                studentResult.SolvedProblems = 0;
                studentResult.Percent = 0;
                studentResult.Rate = 1;
                return;
            }

            int totalQuestionsInTest = studentResult.TaskAnswers.Count(); 
            int correctSolvedQuestions = 0;

            foreach (var taskAnswer in studentResult.TaskAnswers)
            {
                if (taskAnswer.Answer == null || string.IsNullOrWhiteSpace(taskAnswer.Question))
                    continue;

                if (results.TryGetValue(taskAnswer.Question.Trim(), out object correctAnswer))
                {
                   
                    if (AreAnswersEqual(taskAnswer.Answer, correctAnswer, taskAnswer.Question))
                    {
                        correctSolvedQuestions++;
                    }
                }
            }

            studentResult.TotalTask = TaskCount;
            studentResult.SolvedProblems = correctSolvedQuestions;
            studentResult.Percent = totalQuestionsInTest > 0
                ? Math.Round((double)correctSolvedQuestions * 100.0 / TaskCount, 2) 
                : 0;

            studentResult.Rate = GetRate(studentResult.Percent);
        }

        private bool AreAnswersEqual(object studentAnswer, object correctAnswer, string question)
        {
            if (studentAnswer == null && correctAnswer == null)
                return true;

            if (studentAnswer == null || correctAnswer == null)
                return false;

            string studentStr = studentAnswer.ToString().Trim();
            string correctStr = correctAnswer.ToString().Trim();
            if (question.Contains("плотность", StringComparison.OrdinalIgnoreCase))
            {
                return CompareDoubleAnswers(studentStr, correctStr, tolerance: 0.01);
            }
            if (TryParseNumber(studentStr, out double studentNum) &&
                TryParseNumber(correctStr, out double correctNum))
            {
                if (IsInteger(correctNum) && IsInteger(studentNum))
                {
                    return Math.Abs(studentNum - correctNum) < 0.0001;
                }

                return Math.Abs(studentNum - correctNum) < 0.0001;
            }
            return string.Equals(studentStr, correctStr, StringComparison.OrdinalIgnoreCase);
        }

        private bool CompareDoubleAnswers(string studentAnswer, string correctAnswer, double tolerance)
        {
            if (TryParseDouble(studentAnswer, out double studentNum) &&
                TryParseDouble(correctAnswer, out double correctNum))
            {
                return Math.Abs(studentNum - correctNum) <= tolerance;
            }

            return false;
        }

        private bool TryParseDouble(string value, out double result)
        {
            
            value = value?.Replace(" ", "");

            if (string.IsNullOrWhiteSpace(value))
            {
                result = 0;
                return false;
            }
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                return true;
            string withDot = value.Replace(',', '.');
            if (double.TryParse(withDot, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                return true;

            string withComma = value.Replace('.', ',');
            if (double.TryParse(withComma, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                return true;

            return false;
        }

        private bool TryParseNumber(string value, out double result)
        {
            return TryParseDouble(value, out result);
        }

        private bool IsInteger(double number)
        {
            return Math.Abs(number - Math.Round(number)) < 0.0001;
        }

        private int GetRate(double percent)
        {
            return percent switch
            {
                >= 90 => 5,  
                >= 75 => 4, 
                >= 60 => 3,  
                >= 40 => 2,  
                _ => 1        
            };
        }

        public int GetTotalQuestionsInTest()
        {
            return results?.Count ?? 0;
        }

        public Dictionary<string, object> GetCorrectAnswers()
        {
            return results != null
                ? new Dictionary<string, object>(results)
                : new Dictionary<string, object>();
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