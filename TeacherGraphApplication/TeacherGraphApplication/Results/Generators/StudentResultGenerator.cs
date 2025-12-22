
using Generators;
using Filters;
using StudentResultsSpace;
using System.Windows;
using System.Windows.Controls;
using TeacherGraphApplication.Graph;
using System.Collections.Generic;
using System.Linq;

namespace TeacherGraphApplication.Results.Generators
{
    public class StudentResultGenerator
    {
        private Grid _grid;
        private ResultContainer _container;
        private BorderGenerator _borderGenerator;

        public StudentResultGenerator(Grid grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
            _container = new ResultContainer();
            _borderGenerator = new BorderGenerator();
        }

        public void SetGraphContainer(GraphContainer graphContainer)
        {
            if (graphContainer == null)
                throw new ArgumentNullException(nameof(graphContainer));

            _container.SetGraph(graphContainer);
        }

        public bool GraphInitialized()
        {
            return _container.GraphInitialized();
        }

        public void GenerateResults(string path, ISorter? sorter = null)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or empty", nameof(path));

            // Очищаем Grid
            _grid.Children.Clear();
            _grid.RowDefinitions.Clear();
            _grid.ColumnDefinitions.Clear();

            // Загружаем результаты из файла
            _container.LoadFromFile(path);

            try
            {
                // Устанавливаем максимальный индекс для генератора границ
                _borderGenerator.MaxIndex = _container.Count - 1;

                // Получаем список для отображения (сортированный или исходный)
                List<StudentResults> displayResults = sorter != null
                    ? [.. sorter.Sort(_container)]
                    : _container.Students;

                // Если нет результатов для отображения
                if (displayResults.Count == 0)
                {
                    ShowNoResultsMessage();
                    return;
                }

                // Добавляем заголовки столбцов
                AddColumnHeaders();

                // Генерируем строки с результатами
                for (int i = 0; i < displayResults.Count; i++)
                {
                    AddResultRow(i, displayResults[i]);
                }
            }
            catch (Exception ex)
            {
                // Показываем сообщение об ошибке
                ShowErrorMessage($"Ошибка генерации результатов: {ex.Message}");
                throw new ArgumentException($"Ошибка генерации результатов: {ex.Message}", ex);
            }
        }

        private void AddColumnHeaders()
        {
            // Создаем заголовки столбцов
            _grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Pixel) });

            string[] headers = { "Студент", "Правильных ответов", "Процент", "Оценка" };

            for (int i = 0; i < headers.Length; i++)
            {
                if (_grid.ColumnDefinitions.Count <= i)
                {
                    _grid.ColumnDefinitions.Add(new ColumnDefinition());
                }

                var headerBorder = CreateHeaderBorder(headers[i]);
                Grid.SetRow(headerBorder, 0);
                Grid.SetColumn(headerBorder, i);
                _grid.Children.Add(headerBorder);
            }
        }

        private Border CreateHeaderBorder(string text)
        {
            var border = new Border
            {
                BorderBrush = System.Windows.Media.Brushes.Black,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(1),
                Background = System.Windows.Media.Brushes.LightGray
            };

            var textBlock = new TextBlock
            {
                Text = text,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };

            border.Child = textBlock;
            return border;
        }

        private void AddResultRow(int rowIndex, StudentResults result)
        {
            // Добавляем строку для данных
            int dataRowIndex = rowIndex + 1; // +1 из-за заголовка
            _grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60, GridUnitType.Pixel) });

            // Создаем ячейки с данными
            var borders = new[]
            {
                _borderGenerator.GenerateBorder($"{result.Student.LastName} {result.Student.FirstName}", rowIndex),
                _borderGenerator.GenerateBorder(result.SolvedProblems.ToString(), rowIndex),
                _borderGenerator.GenerateBorder($"{result.Percent:P1}", rowIndex), // Форматируем как процент
                _borderGenerator.GenerateBorder(result.Rate.ToString(), rowIndex)
            };

            // Размещаем ячейки в Grid
            for (int colIndex = 0; colIndex < borders.Length; colIndex++)
            {
                Grid.SetRow(borders[colIndex], dataRowIndex);
                Grid.SetColumn(borders[colIndex], colIndex);
                _grid.Children.Add(borders[colIndex]);
            }
        }

        private void ShowNoResultsMessage()
        {
            _grid.RowDefinitions.Add(new RowDefinition());

            var textBlock = new TextBlock
            {
                Text = "Нет результатов для отображения",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 16,
                FontWeight = FontWeights.Bold
            };

            Grid.SetRow(textBlock, 0);
            Grid.SetColumnSpan(textBlock, 4);
            _grid.Children.Add(textBlock);
        }

        private void ShowErrorMessage(string message)
        {
            _grid.RowDefinitions.Add(new RowDefinition());

            var textBlock = new TextBlock
            {
                Text = message,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 14,
                Foreground = System.Windows.Media.Brushes.Red
            };

            Grid.SetRow(textBlock, 0);
            Grid.SetColumnSpan(textBlock, 4);
            _grid.Children.Add(textBlock);
        }

        // Метод для получения статистики по всем студентам
        public Dictionary<string, object> GetStatistics()
        {
            if (_container.Count == 0)
                return new Dictionary<string, object>();

            return new Dictionary<string, object>
            {
                { "Общее количество студентов", _container.Count },
                { "Средний процент выполнения", Math.Round(_container.Students.Average(s => s.Percent), 3) },
                { "Средняя оценка", Math.Round(_container.Students.Average(s => s.Rate), 2) },
                { "Максимальный процент", Math.Round(_container.Students.Max(s => s.Percent), 3) },
                { "Минимальный процент", Math.Round(_container.Students.Min(s => s.Percent), 3) }
            };
        }

        // Метод для получения правильных ответов (для отладки)
        public Dictionary<string, object> GetCorrectAnswers()
        {
            return _container.GetCorrectAnswers();
        }
    }
}
