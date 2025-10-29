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
        private StudentResults studentResults;
        private Student currentStudent;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            InitializeStudent();
        }

        private void InitializeStudent()
        {
            // Здесь можно добавить диалог для ввода имени студента
            // Пока используем тестовые данные
            currentStudent = new Student("Иван", "Иванов");
            studentResults = new StudentResults(currentStudent);
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
            TimerTextBlock.Text = time.ToString(@"hh\:mm\:ss");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SaveResultsToFile();

            TaskPanel.Visibility = Visibility.Collapsed;
            MenuPanel.Visibility = Visibility.Visible;
            timer.Stop();
            timeInSeconds = 0;
            currentConfig = null;
            TimerTextBlock.Text = "00:00:00";
        }

        #region SaveButton
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentAnswer();
            SaveButton.Background = new SolidColorBrush(Colors.White);
        }

        private void SaveCurrentAnswer()
        {
            if (currentConfig?.TaskList == null || currentTaskIndex < 0 || currentTaskIndex >= currentConfig.TaskList.Count)
                return;

            var currentTask = currentConfig.TaskList[currentTaskIndex];
            string studentAnswer = AnswerTextBox.Text;

            if (string.IsNullOrWhiteSpace(studentAnswer))
            {
                MessageBox.Show("Введите ответ перед сохранением", "Пустой ответ",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            studentResults.AddOrUpdateAnswer(currentTask.Content?.ToString() ?? "Без названия", studentAnswer);
            SaveButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4dd49e"));

            MessageBox.Show("Ответ сохранен!", "Сохранение",
                          MessageBoxButton.OK, MessageBoxImage.Information);
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
            if (string.IsNullOrEmpty(currentStudent.FirstName) || string.IsNullOrEmpty(currentStudent.LastName))
            {
                //AskStudentName();
            }

            MenuPanel.Visibility = Visibility.Collapsed;
            TaskPanel.Visibility = Visibility.Visible;
            timer.Start();
            DisplayTasks();
            ShowTask(0);
        }

        #endregion

        #region ExitButton
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            SaveResultsToFile();
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

               
                var taskContent = currentConfig.TaskList[i].Content?.ToString();
                if (!string.IsNullOrEmpty(taskContent) && studentResults.HasAnswerForQuestion(taskContent))
                {
                    button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4dd49e")); 
                }
                else if (i == 0)
                {
                    button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4dd49e")); 
                }
                else
                {
                    button.Background = Brushes.White;
                }

                button.Click += TaskButton_Click;
                taskButtons.Add(button);
                TaskButtonsPanel.Children.Add(button);
            }
            var button2 = new Button
            {
                Content = $"🔚",
                Style = (Style)FindResource("TaskButtonStyle"),
                Tag = 99
            };
            button2.FontSize = 49; 
            button2.FontWeight = FontWeights.Bold;
            button2.Click += TaskButtonExit_Click;
            TaskButtonsPanel.Children.Add(button2);

        }

        private void TaskButtonExit_Click(object sender, RoutedEventArgs e)
        {
            var inputDialog = new Window()
            {
                Title = "Ввод данных студента",
                Width = 400,
                Height = 245,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f0f0f0"))
            };

            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            var contentPanel = new StackPanel
            {
                Margin = new Thickness(20),
                VerticalAlignment = VerticalAlignment.Center
            };

            var label = new TextBlock
            {
                Text = "Введите фамилию и имя:",
                Margin = new Thickness(0, 0, 0, 10),
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = Brushes.Black
            };

            var nameBorder = new Border
            {
                Width = 300,
                Height = 50,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f0f0f0")),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(3),
                Margin = new Thickness(0, 0, 0, 20)
            };

            // Устанавливаем скругленные углы через attached property
            nameBorder.SetValue(ControlAttachedProperties.CornerRadiusProperty, new CornerRadius(20));

            var nameTextBox = new TextBox
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(15, 0, 15, 0),
                TextWrapping = TextWrapping.Wrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Foreground = Brushes.Black,
                CaretBrush = Brushes.Black,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            nameBorder.Child = nameTextBox;

            contentPanel.Children.Add(label);
            contentPanel.Children.Add(nameBorder);

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };

            var okButton = new Button
            {
                Content = "OK",
                Width = 100,
                Height = 40,
                Margin = new Thickness(0, 0, 15, 0),
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4dd49e")),
                Foreground = Brushes.Black,
                BorderThickness = new Thickness(3),
                BorderBrush = Brushes.Black
            };

            var cancelButton = new Button
            {
                Content = "Отмена",
                Width = 100,
                Height = 40,
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#db5174")),
                Foreground = Brushes.Black,
                BorderThickness = new Thickness(3),
                BorderBrush = Brushes.Black
            };

           
            okButton.SetValue(ControlAttachedProperties.CornerRadiusProperty, new CornerRadius(12));
            cancelButton.SetValue(ControlAttachedProperties.CornerRadiusProperty, new CornerRadius(12));

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);

            Grid.SetRow(contentPanel, 0);
            Grid.SetRow(buttonPanel, 1);

            mainGrid.Children.Add(contentPanel);
            mainGrid.Children.Add(buttonPanel);

            inputDialog.Content = mainGrid;

            bool? result = null;

            okButton.Click += (s, e2) =>
            {
                if (!string.IsNullOrWhiteSpace(nameTextBox.Text))
                {
                    result = true;
                    inputDialog.Close();
                }
                else
                {
                    MessageBox.Show("Введите фамилию и имя", "Ошибка");
                }
            };

            cancelButton.Click += (s, e2) =>
            {
                result = false;
                inputDialog.Close();
            };

            okButton.MouseEnter += (s, e) => okButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3bc48d"));
            okButton.MouseLeave += (s, e) => okButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4dd49e"));

            cancelButton.MouseEnter += (s, e) => cancelButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c93a5c"));
            cancelButton.MouseLeave += (s, e) => cancelButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#db5174"));

            nameTextBox.Focus();

            nameTextBox.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    okButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    e.Handled = true;
                }
            };

            inputDialog.ShowDialog();

            if (result == true)
            {
                var nameParts = nameTextBox.Text.Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

                if (nameParts.Length >= 2)
                {
                    currentStudent.LastName = nameParts[0];
                    currentStudent.FirstName = nameParts[1];
                }
                else
                {
                    currentStudent.LastName = nameTextBox.Text.Trim();
                    currentStudent.FirstName = "";
                }

                studentResults.Student = currentStudent;
                SaveResultsToFile();
                Application.Current.Shutdown();
            }
        }

        private void SaveResultsToFile()
        {
            if (studentResults?.TaskAnswers == null || studentResults.TaskAnswers.Count == 0)
                return;

            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(studentResults, options);
                var crypto = new Encryption();
                string encryptedJson = crypto.Encrypt(json);

                var dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.ValidateNames = false;
                dialog.CheckFileExists = false;
                dialog.CheckPathExists = true;
                dialog.FileName = "Select Folder";
                dialog.Title = "Выберите папку для сохранения результатов";

                if (dialog.ShowDialog() == true)
                {
                    string directory = Path.GetDirectoryName(dialog.FileName);
                    string graphResultsPath = Path.Combine(directory, "GraphResults.txt");

                    string resultLine = $"=== {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===\n" +
                                       $"Результаты:\n{encryptedJson}\n" +
                                       new string('=', 50) + "\n";
                    if (File.Exists(graphResultsPath))
                    {
                        File.AppendAllText(graphResultsPath, Environment.NewLine + resultLine);
                    }
                    else
                    {
                        File.WriteAllText(graphResultsPath, resultLine);
                    }

                    MessageBox.Show($"Результаты сохранены в файл: GraphResults.txt", "Сохранение");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении результатов: {ex.Message}", "Ошибка");
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
                var taskContent = currentConfig.TaskList[i].Content?.ToString();
                bool hasAnswer = !string.IsNullOrEmpty(taskContent) && studentResults.HasAnswerForQuestion(taskContent);

                if (i == taskIndex)
                {
                    taskButtons[i].Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4dd49e")); 
                }
                else if (hasAnswer)
                {
                    taskButtons[i].Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4dd49e")); 
                }
                else
                {
                    taskButtons[i].Background = Brushes.White;
                }
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

           
            var savedAnswer = studentResults.GetAnswerForQuestion(task.Content?.ToString());
            AnswerTextBox.Text = savedAnswer?.ToString() ?? "";
        }
        #endregion

       
    }
}