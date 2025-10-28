using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;

namespace StudentGraphApplication
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private int timeInSeconds = 0;
        private ControlWorkConfig currentConfig;
        private List<Button> taskButtons = new List<Button>();
        private int currentTaskIndex = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timeInSeconds++;
            UpdateTimerDisplay();
        }

        private void UpdateTimerDisplay()
        {
            TimeSpan time = TimeSpan.FromSeconds(timeInSeconds);
            // Здесь можно обновить отображение таймера
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            TaskPanel.Visibility = Visibility.Collapsed;
            MenuPanel.Visibility = Visibility.Visible;
            timer.Stop();
            timeInSeconds = 0;
            currentConfig = null;
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

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentConfig == null || currentConfig.TaskList == null || currentConfig.TaskList.Count == 0)
            {
                MessageBox.Show("Сначала загрузите файл с задачами через кнопку 'Файл'", "Нет задач",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MenuPanel.Visibility = Visibility.Collapsed;
            TaskPanel.Visibility = Visibility.Visible;
            timer.Start();

            // Отображаем задачи
            DisplayTasks();
            ShowTask(0); // Показываем первую задачу
        }
        #endregion

        #region ExitButton
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ExitButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ExitButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#db5174"));
        }

        private void ExitButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ExitButton.Background = Brushes.White;
        }
        #endregion

        #region FileButton
        private void FileButton_MouseEnter(object sender, MouseEventArgs e)
        {
            FileButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4dd49e"));
        }

        private void FileButton_MouseLeave(object sender, MouseEventArgs e)
        {
            FileButton.Background = Brushes.White;
        }

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Encrypted files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.Title = "Выберите зашифрованный файл конфигурации";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                currentConfig = ReadEncryptedConfig(filePath);

                if (currentConfig != null)
                {
                    MessageBox.Show($"Успешно загружено: {currentConfig.TaskList?.Count ?? 0} задач",
                                  "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        public ControlWorkConfig? ReadEncryptedConfig(string filePath)
        {
            try
            {
                string encryptedJson = File.ReadAllText(filePath);
                var crypto = new Encryption();
                string decryptedJson = crypto.Decrypt(encryptedJson);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                ControlWorkConfig config = JsonSerializer.Deserialize<ControlWorkConfig>(decryptedJson, options);
                return config;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении файла: {ex.Message}", "Ошибка");
                return null;
            }
        }
        #endregion

        #region Task Management
        private void DisplayTasks()
        {
            taskButtons.Clear();
            TaskButtonsPanel.Children.Clear();
            if (currentConfig?.TaskList == null) return;
            for (int i = 0; i < currentConfig.TaskList.Count; i++)
            {
                var button = new Button
                {
                    Content = $"{i + 1}",
                    Style = (Style)FindResource("TaskButtonStyle"),
                    Tag = i 
                };

                if (i == 0)
                {
                    button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3bc48d"));
                }

                button.Click += TaskButton_Click;
                taskButtons.Add(button);
                TaskButtonsPanel.Children.Add(button);
            }
        }

        private void TaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int taskIndex)
            {
                ShowTask(taskIndex);
            }
        }

        private void ShowTask(int taskIndex)
        {
            if (currentConfig?.TaskList == null || taskIndex < 0 || taskIndex >= currentConfig.TaskList.Count)
                return;

            currentTaskIndex = taskIndex;
            var task = currentConfig.TaskList[taskIndex];
            for (int i = 0; i < taskButtons.Count; i++)
            {
                taskButtons[i].Background = i == taskIndex ?
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4dd49e")) :
                    Brushes.White;
            }

            DisplayTaskContent(task);
        }

        private void DisplayTaskContent(ControlWorkTask task)
        {
            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(10)
            };
            var taskText = new TextBlock
            {
                Text = task.Content?.ToString() ?? "Текст задачи отсутствует",
                FontSize = 18,
                FontWeight = FontWeights.Normal,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 20)
            };

            stackPanel.Children.Add(taskText);
            TaskContentControl.Content = stackPanel;
            AnswerTextBox.Text = "";
        }
        #endregion
    }
}