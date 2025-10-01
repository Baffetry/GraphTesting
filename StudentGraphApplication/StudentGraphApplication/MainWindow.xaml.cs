using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace StudentGraphApplication
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private int timeInSeconds = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);

        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            TaskPanel.Visibility = Visibility.Collapsed;
            MenuPanel.Visibility = Visibility.Visible;
            timer.Stop();
            timeInSeconds = 0;
        }
        #region SaveButton
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveButton.Background = new SolidColorBrush(Colors.White);
        }
        #endregion
        #region StartButton
        private void StartButton_MouseEnter(object sender, MouseEventArgs e)
        {
            StartButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4dd49e"));
        }

        private void StartButton_MouseLeave(object sender, MouseEventArgs e)
        {
            StartButton.Background = Brushes.White;
        }
        #endregion

        #region StartButton
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            MenuPanel.Visibility = Visibility.Collapsed;
            TaskPanel.Visibility = Visibility.Visible;
            timer.Start();
        }
        private void FileButton_MouseEnter(object sender, MouseEventArgs e)
        {
            FileButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4dd49e"));
        }

        private void FileButton_MouseLeave(object sender, MouseEventArgs e)
        {
            FileButton.Background = Brushes.White;
        }
        #endregion

        #region ExitButton
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void ExitButton_MouseEnter(object sender, RoutedEventArgs e)
        {
            ExitButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#db5174"));
        }
        private void ExitButton_MouseLeave(object sender, RoutedEventArgs e)
        {
            ExitButton.Background = Brushes.White;
        }
        #endregion
    }
}