
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Task_Panel_Drawer;
using Graph_Panel_Drawer;

namespace TeacherGraphApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IDrawProperties _propertiesDrower;

        public MainWindow()
        {
            InitializeComponent();
            _propertiesDrower = PropertiesDrawer.SetPropertiesDrower(
                new GraphPanelDrawer(new StackPanelCreator(new GraphDockPanelCreator())),
                new TaskPanelDrawer(new MainDockPanel(new TaskDockPanelCreator()))
            );
        }

        #region ButtonProperties
        private void ButtonProperties_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonProperties.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4dd49e"));
        }
        private void ButtonProperties_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonProperties.Background = Brushes.White;
        }
        private void ButtonProperties_Click(object sender, RoutedEventArgs e)
        {
            PropertiesGrid.Visibility = Visibility.Visible;
            ResultGrid.Visibility = Visibility.Collapsed;

            _propertiesDrower.DrowProperties(PropertiesGrid);
        }
        #endregion

        #region ButtonResults
        private void ButtonResults_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonResults.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4dd49e"));
        }
        private void ButtonResults_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonResults.Background = Brushes.White;
        }
        private void ButtonResults_Click(object sender, RoutedEventArgs e)
        {
            PropertiesGrid.Visibility = Visibility.Collapsed;
            ResultGrid.Visibility = Visibility.Visible;
        }
        #endregion

        #region ButtonExit
        private void ButtonExit_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonExit.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#db5174"));
        }

        private void ButtonExit_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonExit.Background = Brushes.White;
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
    }
}