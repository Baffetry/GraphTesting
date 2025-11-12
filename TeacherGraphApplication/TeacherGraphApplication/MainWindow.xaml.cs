
using BoxContainerSpace;
using Filters;
using Generators;
using Graph_Panel_Drawer;
using Microsoft.Win32;
using Results;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Task_Panel_Drawer;
using TeacherGraphApplication.CWC;
using TeacherGraphApplication.Props.Brusher;
using TeacherGraphApplication.Results.Generators;

namespace TeacherGraphApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

            var generator = new ControlWorkConfigGenerator(container.GetStates(), null);

            if (saveDialog.ShowDialog() is true)
            {
                var temp = new ControlWorkConfig(
                    int.Parse(container.GetTextBox(0).Text),
                    int.Parse(container.GetTextBox(1).Text),
                    generator.GenerateControlWorkConfig()
                );

                var json = JsonSerializer.Serialize(temp, new JsonSerializerOptions {
                    IncludeFields = true,
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
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


            var properties = new PropertiesDrawer(
                new GraphPanelDrawer(),
                new TaskPanelDrawer()
            );

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
            if (string.IsNullOrEmpty(container.GetTextBox(0).Text) ||
                string.IsNullOrEmpty(container.GetTextBox(1).Text))
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
                Title = "Выберите файл"
            };

            if (openFileDialog.ShowDialog() is true)
            {
                resultTableWithStudentsResultsPath = openFileDialog.FileName;
                _tableGenerator.DrawResults(resultTableWithStudentsResultsPath);
                FilterPanel.Visibility = Visibility.Visible;
                ErasePanel.Visibility = Visibility.Visible;
            }
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
            switch (sortByNameState)
            {
                case SortState.Default:
                    _tableGenerator.DrawResults(resultTableWithStudentsResultsPath);
                    sortByNameState = SortState.Ascending;
                    break;

                case SortState.Ascending:
                    _tableGenerator.DrawResults(resultTableWithStudentsResultsPath,
                        new SortByName(true));

                    sortByNameState = SortState.Descending;
                    break;

                case SortState.Descending:
                    _tableGenerator.DrawResults(resultTableWithStudentsResultsPath,
                        new SortByName(false));

                    sortByNameState = SortState.Default;
                    break;
            }
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
            switch (sortBySPState)
            {
                case SortState.Default:
                    _tableGenerator.DrawResults(resultTableWithStudentsResultsPath);
                    sortBySPState = SortState.Ascending;
                    break;

                case SortState.Ascending:
                    _tableGenerator.DrawResults(resultTableWithStudentsResultsPath,
                        new SortBySolvedProblems(true));

                    sortBySPState = SortState.Descending;
                    break;

                case SortState.Descending:
                    _tableGenerator.DrawResults(resultTableWithStudentsResultsPath,
                        new SortBySolvedProblems(false));

                    sortBySPState = SortState.Default;
                    break;
            }
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
            switch (sortByPercentState)
            {
                case SortState.Default:
                    _tableGenerator.DrawResults(resultTableWithStudentsResultsPath);
                    sortByPercentState = SortState.Ascending;
                    break;

                case SortState.Ascending:
                    _tableGenerator.DrawResults(resultTableWithStudentsResultsPath,
                        new SortByPercent(true));

                    sortByPercentState = SortState.Descending;
                    break;

                case SortState.Descending:
                    _tableGenerator.DrawResults(resultTableWithStudentsResultsPath,
                        new SortByPercent(false));

                    sortByPercentState = SortState.Default;
                    break;
            }
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
            switch (sortByRateState)
            {
                case SortState.Default:
                    _tableGenerator.DrawResults(resultTableWithStudentsResultsPath);
                    sortByRateState = SortState.Ascending;
                    break;

                case SortState.Ascending:
                    _tableGenerator.DrawResults(resultTableWithStudentsResultsPath,
                        new SortByRate(true));

                    sortByRateState = SortState.Descending;
                    break;

                case SortState.Descending:
                    _tableGenerator.DrawResults(resultTableWithStudentsResultsPath,
                        new SortByRate(false));

                    sortByRateState = SortState.Default;
                    break;
            }
        }
        #endregion

        #endregion
    }
}