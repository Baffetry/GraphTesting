
using Graph_Panel_Drawer;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Task_Panel_Drawer;
using TeacherGraphApplication.CWC;
using TeacherGraphApplication.Props.Brusher;
using BoxContainerSpace;
using System.Text.Json;
using System.IO;

namespace TeacherGraphApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IContainer container;
        private Brusher brush;

        public MainWindow()
        {
            InitializeComponent();

            this.Closing += MainWindow_Closing; // Событие при закрытии окна

            brush = new Brusher();
            container = BoxContainer.Instance();
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
        //private void LoadWithFileDialog()
        //{
        //    //var openDialog = new OpenFileDialog 
        //    //{
        //    //    Filter = "JSON files (*.json)|*.json",
        //    //    DefaultExt = ".json"
        //    //};


        //    //if (openDialog.ShowDialog() is true)
        //    //    container.Load(openDialog.FileName);
        //}
        private bool SaveWithFileDialog(string fileName)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                DefaultExt = ".json",
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

                File.WriteAllText(saveDialog.FileName, json);

                return true;
            }

            return false;
        }
        #endregion

        #region ButtonProperties
        private void ButtonProperties_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonProperties.Background = brush.Enable;
        }
        private void ButtonProperties_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ButtonProperties.IsEnabled)
                ButtonProperties.Background = brush.Disable;
        }
        private void ButtonProperties_Click(object sender, RoutedEventArgs e)
        {
            PropertiesGrid.Visibility = Visibility.Visible;
            PropButtons.Visibility = Visibility.Visible;
            ResultGrid.Visibility = Visibility.Collapsed;

            ButtonProperties.IsEnabled = false;
            ButtonResults.IsEnabled = true;

            ButtonProperties.Background = brush.Enable;
            ButtonResults.Background = brush.Disable;

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
            ButtonResults.Background = brush.Enable;
        }
        private void ButtonResults_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ButtonResults.IsEnabled)
                ButtonResults.Background = brush.Disable;
        }
        private void ButtonResults_Click(object sender, RoutedEventArgs e)
        {
            PropertiesGrid.Visibility = Visibility.Collapsed;
            PropButtons.Visibility = Visibility.Collapsed;
            ResultGrid.Visibility = Visibility.Visible;

            ButtonProperties.IsEnabled = true;
            ButtonResults.IsEnabled = false;

            ButtonProperties.Background = brush.Disable;
            ButtonResults.Background = brush.Enable;

            container.Save();
        }
        #endregion

        #region ButtonExit
        private void ButtonExit_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonExit.Background = brush.Exit;
        }

        private void ButtonExit_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonExit.Background = brush.Disable;
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        #region ScrollBar
        private void ScrollViewer_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;

            if (scrollViewer.ExtentHeight > scrollViewer.ViewportHeight)
                Labels.Margin = new Thickness(0, 0, 17, 0);
            else
                Labels.Margin = new Thickness(0, 0, 0, 0);
        }
        #endregion

        #region ButtonSave
        private void ButtonSave_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonSave.Background = brush.Enable;

            ButtonSave.Height = 120;
            ButtonSave.Width = 500;
            ButtonSave.FontSize = 44;
        }

        private void ButtonSave_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonSave.Background = brush.Disable;

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
            ButtonReset.Background = brush.Exit;
            ButtonReset.Height = 120;
            ButtonReset.Width = 120;

            ResetButtonIcon.Height = 90;
            ResetButtonIcon.Width = 90;
        }

        private void ButtonReset_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonReset.Background = brush.Disable;
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
    }
}