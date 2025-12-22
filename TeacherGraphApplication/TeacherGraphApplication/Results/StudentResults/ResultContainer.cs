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

        public ResultContainer() { }

        public int Count
            => students.Count;

        public List<StudentResults> Students
            => students;

        public StudentResults this[int index]
            => students[index];

        public void SetGraph(GraphContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            graph = new GraphCalc(container);

            // Вычисляем все результаты один раз при инициализации графа
            results = new Dictionary<string, object>()
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
            return graph != null && results != null;
        }

        public void LoadFromFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or empty", nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}");

            // Очищаем старые данные
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

            int totalQuestionsInTest = results.Count; // Количество вопросов, которые задал учитель
            int correctSolvedQuestions = 0;

            foreach (var taskAnswer in studentResult.TaskAnswers)
            {
                if (taskAnswer.Answer == null || string.IsNullOrWhiteSpace(taskAnswer.Question))
                    continue;

                // Ищем правильный ответ в словаре results
                if (results.TryGetValue(taskAnswer.Question.Trim(), out object correctAnswer))
                {
                    // Сравниваем ответы с учетом разных типов данных
                    if (AreAnswersEqual(taskAnswer.Answer, correctAnswer, taskAnswer.Question))
                    {
                        correctSolvedQuestions++;
                    }
                }
            }

            studentResult.SolvedProblems = correctSolvedQuestions;

            // Процент правильно решенных от общего количества вопросов в тесте
            studentResult.Percent = totalQuestionsInTest > 0
                ? Math.Round((double)correctSolvedQuestions / totalQuestionsInTest, 3)
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

            // Специальная обработка для плотности графа (и других возможных дробных чисел)
            if (question.Contains("плотность", StringComparison.OrdinalIgnoreCase))
            {
                return CompareDoubleAnswers(studentStr, correctStr, tolerance: 0.01);
            }

            // Пытаемся сравнить как числа (целые или дробные)
            if (TryParseNumber(studentStr, out double studentNum) &&
                TryParseNumber(correctStr, out double correctNum))
            {
                // Для целых чисел сравниваем точно
                if (IsInteger(correctNum) && IsInteger(studentNum))
                {
                    return Math.Abs(studentNum - correctNum) < 0.0001;
                }

                // Для дробных чисел (как плотность) сравниваем с погрешностью
                return Math.Abs(studentNum - correctNum) < 0.0001;
            }

            // Сравниваем как строки
            return string.Equals(studentStr, correctStr, StringComparison.OrdinalIgnoreCase);
        }

        private bool CompareDoubleAnswers(string studentAnswer, string correctAnswer, double tolerance)
        {
            // Пытаемся распарсить оба ответа как числа, заменяя запятую на точку
            if (TryParseDouble(studentAnswer, out double studentNum) &&
                TryParseDouble(correctAnswer, out double correctNum))
            {
                return Math.Abs(studentNum - correctNum) <= tolerance;
            }

            return false;
        }

        private bool TryParseDouble(string value, out double result)
        {
            // Пробуем несколько форматов:
            // 1. Сначала как есть (может быть "0.2" или "0,2")
            // 2. Заменяем запятую на точку
            // 3. Заменяем точку на запятую

            // Убираем пробелы
            value = value?.Replace(" ", "");

            if (string.IsNullOrWhiteSpace(value))
            {
                result = 0;
                return false;
            }

            // Пробуем стандартный парсинг (работает для "0.2" в invariant culture)
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                return true;

            // Пробуем с заменой запятой на точку
            string withDot = value.Replace(',', '.');
            if (double.TryParse(withDot, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                return true;

            // Пробуем с заменой точки на запятую
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
            // Шкала оценок от общего количества вопросов в тесте
            return percent switch
            {
                >= 0.9 => 5,  // 90-100% - отлично
                >= 0.75 => 4, // 75-89% - хорошо
                >= 0.6 => 3,  // 60-74% - удовлетворительно
                >= 0.4 => 2,  // 40-59% - неудовлетворительно
                _ => 1        // 0-39% - плохо
            };
        }

        // Получить количество вопросов, которые задал учитель
        public int GetTotalQuestionsInTest()
        {
            return results?.Count ?? 0;
        }

        // Метод для тестирования - получение правильных ответов
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