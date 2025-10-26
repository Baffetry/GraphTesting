using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TeacherGraphApplication
{
    /// <summary>
    /// Interaction logic for CustomMessageBoxWindow.xaml
    /// </summary>
    /// 

    public enum MessageType
    {
        Warning,
        Error
    }

    public partial class CustomMessageBox: Window
    {
        public MessageBoxResult result { get; set; }

        public CustomMessageBox(string message, string title, MessageType type, MessageBoxButton button)
        {
            InitializeComponent();

            SetIcon(type);

            TitleLabel.Content = title;
            ContentLabel.Content = message;

            switch (button)
            {
                case MessageBoxButton.YesNo:

                    YesNoPanel.Visibility = Visibility.Visible;
                    
                    ButtonYes.Click += (s, e) => { result = MessageBoxResult.Yes; Close(); };
                    ButtonNo.Click += (s, e) => { result = MessageBoxResult.No; Close(); };

                    break;

                case MessageBoxButton.OK:

                    OkPanel.Visibility = Visibility.Visible;

                    ButtonOk.Click += (s, e) => { result = MessageBoxResult.OK; Close(); };
                    break;

                default:
                    break;
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        public static MessageBoxResult Show(string message, string title, MessageType type, MessageBoxButton button)
        {
            var messageBox = new CustomMessageBox(message, title, type, button);
            messageBox.ShowDialog();
            return messageBox.result;
        }

        private void SetIcon(MessageType type)
        {
            string iconPath = type switch
            {
                MessageType.Warning => "/Resources/warning32x32_black.png",
                _ => "/Resources/warning32x32_black.png"
            };

            IconImage.Source = new BitmapImage(new Uri(iconPath, UriKind.Relative));
        }

        #region Button close
        private void ButtonClose_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonClose.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#db5174"));
        }
        private void ButtonClose_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonClose.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f0f0f0"));
        }
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            result = MessageBoxResult.Cancel; Close();
        }
        #endregion
    }
}
