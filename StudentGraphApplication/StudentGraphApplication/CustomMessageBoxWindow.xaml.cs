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

namespace StudentGraphApplication
{
    /// <summary>
    /// Логика взаимодействия для CustomMessageBoxWindow.xaml
    /// </summary>
    /// 

    public enum MessageType
    {
        Warning,
        Error
    }
    public partial class CustomMessageBoxWindow : Window
    {
        public MessageBoxResult result { get; set; }

        public CustomMessageBoxWindow(string message, string title, MessageType type, MessageBoxButton button)
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
            var messageBox = new CustomMessageBoxWindow(message, title, type, button);
            messageBox.ShowDialog();
            return messageBox.result;
        }

        private void SetIcon(MessageType type)
        {
            string iconPath = type switch
            {
                MessageType.Warning => "/Resources/warning96x96.png",
                _ => "/Resources/warning96x96.png"
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

        #region ButtonYes
        private void ButtonYes_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonYes.Height = 60;
            ButtonYes.Width = 70;
            ButtonYes.FontSize = 22;
        }

        private void ButtonYes_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonYes.Height = 50;
            ButtonYes.Width = 60;
            ButtonYes.FontSize = 18;
        }
        #endregion

        #region ButtonNo
        private void ButtonNo_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonNo.Height = 60;
            ButtonNo.Width = 70;
            ButtonNo.FontSize = 22;
        }

        private void ButtonNo_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonNo.Height = 50;
            ButtonNo.Width = 60;
            ButtonNo.FontSize = 18;
        }
        #endregion

        #region ButtonOk
        private void ButtonOk_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonOk.Height = 60;
            ButtonOk.Width = 70;
            ButtonOk.FontSize = 22;
        }

        private void ButtonOk_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonOk.Height = 50;
            ButtonOk.Width = 60;
            ButtonOk.FontSize = 18;
        }
        #endregion
    }
}
