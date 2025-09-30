using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace TeacherGraphApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
    }
}