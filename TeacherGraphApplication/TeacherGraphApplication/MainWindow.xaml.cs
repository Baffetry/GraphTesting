
using BoxContainerSpace;
using Filters;
using Generators;
using GraphShape.Controls;
using Microsoft.Win32;
using QuikGraph;
using Results;
using StudentGraphApplication;
using StudentResultsSpace;
using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TeacherGraphApplication.CWC;
using TeacherGraphApplication.Graph;
using TeacherGraphApplication.Props.Brusher;
using TeacherGraphApplication.Results.Generators;

namespace TeacherGraphApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        public Random _random = new Random();
        private GraphContainer graphContainer;

        private IContainer container;
        private ITableGenerator _tableGenerator;
        private ScrollViewer _scrollViewer;
        private Brusher brush;

        private string resultTableWithStudentsResultsPath;

        private SortState sortByNameState = SortState.Ascending;
        private SortState sortBySPState = SortState.Ascending;
        private SortState sortByPercentState = SortState.Ascending;
        private SortState sortByRateState = SortState.Ascending;

        public enum SortState
        {
            Default,
            Ascending,
            Descending
        }

        public MainWindow()
        {
            InitializeComponent();

            this.Closing += MainWindow_Closing; // Событие при закрытии окна

            brush = new Brusher();
            container = BoxContainer.Instance();
            _tableGenerator = TableGenerator.Instance(new GridGenerator(Labels),
                new StudentResultGenerator(ResultTable));

            _scrollViewer = FindName("ResultViewer") as ScrollViewer;
        }

        #region Graph methods
        private void CreateGraphWithRandomPositions(int amountOfVertex, int amountOfEdge)
        {
            int C_n = (amountOfVertex * (amountOfVertex - 1)) / 2;

            int maxEdges = amountOfEdge <= C_n
                ? amountOfEdge
                : C_n;

            graphContainer.Clear();

            // Используем object как тип вершин для совместимости
            var graph = new BidirectionalGraph<object, IEdge<object>>();

            // Создаем вершины со случайными позициями
            var vertices = new List<GraphVertex>();

            for (int i = 1; i <= amountOfVertex; i++)
            {
                var vertex = new GraphVertex($"{i}")
                {
                    X = _random.Next(10, (int)GraphBorder.Width - 50),
                    Y = _random.Next(10, (int)GraphBorder.Height - 50)
                };
                graphContainer.AddVertex(vertex);
            }

            AddRandomEdgesToContainer(graphContainer, maxEdges);

            // Устанавливаем граф
            graphLayout.Graph = graphContainer.ToGraphLayoutGraph();
            graphLayout.LayoutAlgorithmType = "None"; // Отключаем авто-компоновку

            // Устанавливаем позиции вершин
            SetVertexPositions(graphContainer.Vertices);
        }
        private void AddRandomEdgesToContainer(GraphContainer container, int amountOfEdges)
        {
            var vertices = container.Vertices;
            var existingEdges = new HashSet<(int, int)>();

            while (amountOfEdges > 0)
            {
                int sourceIndex = _random.Next(vertices.Count);
                int targetIndex = _random.Next(vertices.Count);

                if (sourceIndex == targetIndex)
                    continue;

                // Создаем ключ для проверки уникальности ребра
                int minIndex = Math.Min(sourceIndex, targetIndex);
                int maxIndex = Math.Max(sourceIndex, targetIndex);
                var edgeKey = (minIndex, maxIndex);

                if (!existingEdges.Contains(edgeKey))
                {
                    var sourceVertex = vertices[sourceIndex];
                    var targetVertex = vertices[targetIndex];

                    // Добавляем ребро через контейнер
                    container.AddEdge(sourceVertex, targetVertex);

                    existingEdges.Add(edgeKey);
                    amountOfEdges--;
                }
            }
        }
        private void SetVertexPositions(List<GraphVertex> vertices)
        {
            foreach (var vertex in vertices)
            {
                var vertexControl = graphLayout.GetVertexControl(vertex);
                if (vertexControl != null)
                {
                    GraphCanvas.SetX(vertexControl, vertex.X);
                    GraphCanvas.SetY(vertexControl, vertex.Y);
                }
            }
        }

        #endregion

        #region MainWindow event
        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!container.IsChanged()) return;

            var result = CustomMessageBox.Show(
                "Хотите сохранить параметры контрольной работы?",
                "Предупреждение",
                MessageType.Warning,
                MessageBoxButton.YesNo
            );

            if (result is MessageBoxResult.Yes)
                e.Cancel = !SaveWithFileDialog("Template");
            else if (result is MessageBoxResult.Cancel || result is MessageBoxResult.None)
                e.Cancel = true;
        }
        #endregion

        #region Functional methods of the main window
        private bool SaveWithFileDialog(string fileName)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.txt)|*.txt",
                DefaultExt = ".txt",
                FileName = fileName
            };

            var generator = new ControlWorkConfigGenerator(container.GetStates());

            if (saveDialog.ShowDialog() is true)
            {
                var temp = new ControlWorkConfig {
                    Vertices = graphContainer.Vertices.Count(),
                    Edges = graphContainer.Edges.Count(),
                    TaskList = generator.GenerateControlWorkConfig(),
                    Container = graphContainer
                };

                var json = JsonSerializer.Serialize(temp, new JsonSerializerOptions {
                    IncludeFields = true,
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                File.WriteAllText(saveDialog.FileName + "JSON", json);


                var encprition = new Encryption();
                string result = encprition.Encrypt(json);
                File.WriteAllText(saveDialog.FileName, result);

                return true;
            }

            return false;
        }
        private List<Border> GetColumnElementsFromResultTable(int columnIndex)
        {
            List<Border> borders = new List<Border>();

            foreach (UIElement child in ResultTable.Children)
                if (child is Border border && Grid.GetColumn(border) == columnIndex)
                    borders.Add(border);

            return borders;
        }
        private void ChangeBordersBackgroundInResultTable(int index, SolidColorBrush colorBrush)
        {
            var borders = GetColumnElementsFromResultTable(index);
            foreach (var border in borders)
                border.Background = colorBrush;
        }
        private void UseSort(ref SortState state, Filter filter)
        {
            switch (state)
            {
                case SortState.Default:
                    _tableGenerator.DrawResults(resultTableWithStudentsResultsPath);
                    state = SortState.Ascending;
                    break;

                case SortState.Ascending:

                    filter.Ascending = true;
                    _tableGenerator.DrawResults(resultTableWithStudentsResultsPath,
                        filter);

                    state = SortState.Descending;
                    break;

                case SortState.Descending:

                    filter.Ascending = false;
                    _tableGenerator.DrawResults(resultTableWithStudentsResultsPath,
                        filter);

                    state = SortState.Default;
                    break;
            }
        }
        #endregion

        #region ScrollViewer
        private void ScrollViewer_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;

            if (scrollViewer.ExtentHeight > scrollViewer.ViewportHeight)
                Labels.Margin = new Thickness(0, 0, 17, 0);
            else
                Labels.Margin = new Thickness(0, 0, 0, 0);
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_tableGenerator != null)
            {
                _scrollViewer.ScrollToVerticalOffset(_scrollViewer.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }
        #endregion

        #region Buttons

        #region ButtonProperties
        private void ButtonProperties_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonProperties.Background = brush.CandyGreen;
        }
        private void ButtonProperties_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ButtonProperties.IsEnabled)
                ButtonProperties.Background = brush.CandyGray;
        }
        private void ButtonProperties_Click(object sender, RoutedEventArgs e)
        {
            // Props
            PropertiesGrid.Visibility = Visibility.Visible;
            PropButtons.Visibility = Visibility.Visible;
            GraphGeneratePanel.Visibility = Visibility.Visible;
            GraphPropPanel.Visibility = Visibility.Visible;

            // Results
            ResultGrid.Visibility = Visibility.Collapsed;
            OpenFilePanel.Visibility = Visibility.Collapsed;
            FilterPanel.Visibility = Visibility.Collapsed;
            ErasePanel.Visibility = Visibility.Collapsed;

            ButtonProperties.IsEnabled = false;
            ButtonResults.IsEnabled = true;

            ButtonProperties.Background = brush.CandyGreen;
            ButtonResults.Background = brush.CandyGray;

            container.Load();


            var properties = new PropertiesDrawer();

            properties.Draw(PropertiesGrid);
        }
        #endregion

        #region ButtonResults
        private void ButtonResults_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonResults.Background = brush.CandyGreen;
        }
        private void ButtonResults_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ButtonResults.IsEnabled)
                ButtonResults.Background = brush.CandyGray;
        }
        private void ButtonResults_Click(object sender, RoutedEventArgs e)
        {
            // Props
            PropertiesGrid.Visibility = Visibility.Collapsed;
            PropButtons.Visibility = Visibility.Collapsed;
            GraphGeneratePanel.Visibility = Visibility.Collapsed;
            GraphPropPanel.Visibility = Visibility.Collapsed;

            // Results
            ResultGrid.Visibility = Visibility.Visible;
            OpenFilePanel.Visibility = Visibility.Visible;

            ButtonProperties.IsEnabled = true;
            ButtonResults.IsEnabled = false;

            ButtonProperties.Background = brush.CandyGray;
            ButtonResults.Background = brush.CandyGreen;

            container.Save();

            _tableGenerator.DrawLabels();

            FilterPanel.Visibility = ResultTable.Children.Count > 0
                ? Visibility.Visible
                : Visibility.Collapsed;

            ErasePanel.Visibility = ResultTable.Children.Count > 0
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        #endregion

        #region ButtonExit
        private void ButtonExit_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonExit.Background = brush.CandyRed;
        }
        private void ButtonExit_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonExit.Background = brush.CandyGray;
        }
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        #region ButtonSave
        private void ButtonSave_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonSave.Background = brush.CandyGreen;

            ButtonSave.Height = 120;
            ButtonSave.Width = 500;
            ButtonSave.FontSize = 44;
        }
        private void ButtonSave_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonSave.Background = brush.CandyGray;

            ButtonSave.Height = 110;
            ButtonSave.Width = 490;
            ButtonSave.FontSize = 40;
        }
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(VertexTB.Text) ||
                string.IsNullOrEmpty(EdgeTB.Text))
            {
                var result = CustomMessageBox.Show(
                    "Проверьте, правильно ли вы ввели кол-во рёбер или вершин.",
                    "Предупреждение",
                    MessageType.Warning,
                    MessageBoxButton.OK
                );

                return;
            }

            container.Update();
            SaveWithFileDialog("ControlWorkConfig");
        }
        #endregion

        #region ButtonReset
        private void ButtonReset_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonReset.Background = brush.CandyRed;
            ButtonReset.Height = 120;
            ButtonReset.Width = 120;

            ResetButtonIcon.Height = 90;
            ResetButtonIcon.Width = 90;
        }
        private void ButtonReset_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonReset.Background = brush.CandyGray;
            ButtonReset.Height = 110;
            ButtonReset.Width = 110;

            ResetButtonIcon.Height = 70;
            ResetButtonIcon.Width = 70;
        }
        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            (VertexTB.Text, EdgeTB.Text) = (string.Empty, string.Empty);
            graphLayout.Graph = new BidirectionalGraph<object, IEdge<object>>();
            graphContainer = null;

            container.Stop();
        }
        #endregion

        #region ButtonOpenFile
        private void ButtonOpenFile_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonOpenFile.Background = brush.CandyGreen;
            ButtonOpenFile.Height = 120;
            ButtonOpenFile.Width = 120;

            OpenFileI.Height = 90;
            OpenFileI.Width = 90;
        }
        private void ButtonOpenFile_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonOpenFile.Background = brush.CandyGray;
            ButtonOpenFile.Height = 110;
            ButtonOpenFile.Width = 110;

            OpenFileI.Height = 70;
            OpenFileI.Width = 70;
        }
        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            { 
                Filter = "(*.txt)|*.txt",
                Title = "Выберите файл с результатами студентов. . ."
            };

            var targetCfg = OpenTargetCfg(openFileDialog);
            TableGenerator.SetGraph(targetCfg.Container);

            if (openFileDialog.ShowDialog() is true)
            {
                resultTableWithStudentsResultsPath = openFileDialog.FileName;
                _tableGenerator.DrawResults(resultTableWithStudentsResultsPath);
                FilterPanel.Visibility = Visibility.Visible;
                ErasePanel.Visibility = Visibility.Visible;
            }
        }

        private ControlWorkConfig OpenTargetCfg(OpenFileDialog opf)
        {
            opf.Title = "Выберите варинт контрольной работы. . .";
            ControlWorkConfig target = null;

            if (opf.ShowDialog() is true)
            {
                string temp = File.ReadAllText(opf.FileName);
                var decrypt = new Encryption();
                var json = decrypt.Decrypt(temp);
                target = JsonSerializer.Deserialize<ControlWorkConfig>(json);
            }

            return target;
        }
        #endregion

        #region ButtonErase
        private void ButtonEraseTable_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonEraseTable.Background = brush.CandyRed;
            ButtonEraseTable.Height = 120;
            ButtonEraseTable.Width = 120;

            EraseI.Height = 90;
            EraseI.Width = 90;
        }
        private void ButtonEraseTable_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonEraseTable.Background = brush.CandyGray;
            ButtonEraseTable.Height = 110;
            ButtonEraseTable.Width = 110;

            EraseI.Height = 70;
            EraseI.Width = 70;
        }
        private void ButtonEraseTable_Click(object sender, RoutedEventArgs e)
        {
            ResultTable.Children.Clear();
            ResultTable.RowDefinitions.Clear();

            FilterPanel.Visibility = Visibility.Collapsed;
            ErasePanel.Visibility = Visibility.Collapsed;

            sortByNameState = SortState.Ascending;
            sortBySPState = SortState.Ascending;
            sortByPercentState = SortState.Ascending;
            sortByRateState = SortState.Ascending;
        }
        #endregion

        #region ButtonGraphGenerate
        private void GraphGenerateButton_MouseEnter(object sender, MouseEventArgs e)
        {
            GraphGenerateButton.Background = brush.CandyGreen;
            GraphGenerateButton.Height = 120;
            GraphGenerateButton.Width = 120;

            GraphIcon.Height = 90;
            GraphIcon.Width = 90;
        }

        private void GraphGenerateButton_MouseLeave(object sender, MouseEventArgs e)
        {
            GraphGenerateButton.Background = brush.CandyGray;
            GraphGenerateButton.Height = 110;
            GraphGenerateButton.Width = 110;

            GraphIcon.Height = 70;
            GraphIcon.Width = 70;
        }

        private void GraphGenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(VertexTB.Text) || string.IsNullOrEmpty(EdgeTB.Text))
            {
                var result = CustomMessageBox.Show(
                        "Некорректные параметры графа!",
                        "Предупреждение",
                        MessageType.Warning,
                        MessageBoxButton.OK
                );

                if (result is MessageBoxResult.OK || result is MessageBoxResult.Cancel || result is MessageBoxResult.None)
                    return;
            }

            graphContainer = new GraphContainer();
            //TableGenerator.SetGraph(graphContainer);

            int vertices = int.Parse(VertexTB.Text);
            int edges = int.Parse(EdgeTB.Text);

            CreateGraphWithRandomPositions(vertices, edges);
        }
        #endregion

        #endregion

        #region Filters

        #region ButtonNameFilter
        private void ButtonNameFilter_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.AddPreviewMouseWheelHandler(this, ScrollViewer_PreviewMouseWheel);

            ButtonNameFilter.Background = brush.CandyBlue;
            ButtonNameFilter.Height = 120;
            ButtonNameFilter.Width = 120;

            FByPerson.Height = 90;
            FByPerson.Width = 90;

            ChangeBordersBackgroundInResultTable(0, brush.CandyBlue);
        }
        private void ButtonNameFilter_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.RemovePreviewMouseWheelHandler(this, ScrollViewer_PreviewMouseWheel);

            ButtonNameFilter.Background = brush.CandyGray;
            ButtonNameFilter.Height = 110;
            ButtonNameFilter.Width = 110;

            FByPerson.Height = 70;
            FByPerson.Width = 70;
            
            ChangeBordersBackgroundInResultTable(0, brush.CandyGray);
        }
        private void ButtonNameFilter_Click(object sender, RoutedEventArgs e)
        {
            UseSort(ref sortByNameState, new SortByName());
        }
        #endregion

        #region ButtonSPFilter
        private void ButtonSPFilter_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.AddPreviewMouseWheelHandler(this, ScrollViewer_PreviewMouseWheel);

            ButtonSPFilter.Background = brush.CandyBlue;
            ButtonSPFilter.Height = 120;
            ButtonSPFilter.Width = 120;

            FBySP.Height = 90;
            FBySP.Width = 90;

            ChangeBordersBackgroundInResultTable(1, brush.CandyBlue);
        }
        private void ButtonSPFilter_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.RemovePreviewMouseWheelHandler(this, ScrollViewer_PreviewMouseWheel);

            ButtonSPFilter.Background = brush.CandyGray;
            ButtonSPFilter.Height = 110;
            ButtonSPFilter.Width = 110;

            FBySP.Height = 70;
            FBySP.Width = 70;

            ChangeBordersBackgroundInResultTable(1, brush.CandyGray);
        }
        private void ButtonSPFilter_Click(object sender, RoutedEventArgs e)
        {
            UseSort(ref sortBySPState, new SortBySolvedProblems());
        }
        #endregion

        #region ButtonPercentFilter
        private void ButtonPercentFilter_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.AddPreviewMouseWheelHandler(this, ScrollViewer_PreviewMouseWheel);

            ButtonPercentFilter.Background = brush.CandyBlue;
            ButtonPercentFilter.Height = 120;
            ButtonPercentFilter.Width = 120;

            FByPercent.Height = 90;
            FByPercent.Width = 90;

            ChangeBordersBackgroundInResultTable(2, brush.CandyBlue);
        }
        private void ButtonPercentFilter_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.RemovePreviewMouseWheelHandler(this, ScrollViewer_PreviewMouseWheel);

            ButtonPercentFilter.Background = brush.CandyGray;
            ButtonPercentFilter.Height = 110;
            ButtonPercentFilter.Width = 110;

            FByPercent.Height = 70;
            FByPercent.Width = 70;

            ChangeBordersBackgroundInResultTable(2, brush.CandyGray);
        }
        private void ButtonPercentFilter_Click(object sender, RoutedEventArgs e)
        {
            UseSort(ref sortByPercentState, new SortByPercent());
        }

        #endregion

        #region ButtonRateFilter
        private void ButtonRateFilter_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.AddPreviewMouseWheelHandler(this, ScrollViewer_PreviewMouseWheel);

            ButtonRateFilter.Background = brush.CandyBlue;
            ButtonRateFilter.Height = 120;
            ButtonRateFilter.Width = 120;

            FByRate.Height = 110;
            FByRate.Width = 110;

            ChangeBordersBackgroundInResultTable(3, brush.CandyBlue);
        }
        private void ButtonRateFilter_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.RemovePreviewMouseWheelHandler(this, ScrollViewer_PreviewMouseWheel);

            ButtonRateFilter.Background = brush.CandyGray;
            ButtonRateFilter.Height = 110;
            ButtonRateFilter.Width = 110;

            FByRate.Height = 90;
            FByRate.Width = 90;

            ChangeBordersBackgroundInResultTable(3, brush.CandyGray);
        }
        private void ButtonRateFilter_Click(object sender, RoutedEventArgs e)
        {
            UseSort(ref sortByRateState, new SortByRate());
        }
        #endregion

        #endregion

        #region Graph Text Boxes
        private void VertexTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        private void EdgeTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0); 
        }
        #endregion
    }
}