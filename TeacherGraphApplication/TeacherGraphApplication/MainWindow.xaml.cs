
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Task_Panel_Drawer;
using Graph_Panel_Drawer;
using TeacherGraphApplication.Props.TaskPanelDrawer.Generators.Container;
using Microsoft.Win32;
using TeacherGraphApplication.Props.Brusher;

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
                MessageType.Warning
            );

            if (result is MessageBoxResult.Yes)
                e.Cancel = !SaveWithFileDialog();
            else if (result is MessageBoxResult.Cancel || result is MessageBoxResult.None)
                e.Cancel = true;

            container.Stop();
        }
        #endregion

        #region Functional methods of the main window
        private void LoadWithFileDialog()
        {
            var openDialog = new OpenFileDialog 
            {
                Filter = "JSON files (*.json)|*.json",
                DefaultExt = ".json"
            };


            if (openDialog.ShowDialog() is true)
                container.Load(openDialog.FileName);
        }
        private bool SaveWithFileDialog()
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                DefaultExt = ".json",
                FileName = "ControlWork"
            };

            if (saveDialog.ShowDialog() is true)
            {
                container.Save(saveDialog.FileName);
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
                new GraphPanelDrawer(new StackPanelCreator(new GraphDockPanelCreator())),
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

        #region Button save variant
        private void ButtonSaveVariant_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonSave.Background = brush.Enable;
        }

        private void ButtonSaveVariant_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonSave.Background = brush.Disable;
        }

        private void ButtonSaveVariant_Click(object sender, RoutedEventArgs e)
        {
            SaveWithFileDialog();
        }
        #endregion
    }
}