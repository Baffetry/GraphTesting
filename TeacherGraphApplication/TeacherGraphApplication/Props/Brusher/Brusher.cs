using System.Windows.Controls;
using System.Windows.Media;

namespace TeacherGraphApplication.Props.Brusher
{
    internal class Brusher
    {
        private SolidColorBrush candyGray = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ededed"));
        private SolidColorBrush candyGreen = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4dd49e"));
        private SolidColorBrush candyRed = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#db5174"));
        private SolidColorBrush candyBlue = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#82e1fe"));

        public SolidColorBrush CandyGray => candyGray;
        public SolidColorBrush CandyGreen => candyGreen;
        public SolidColorBrush CandyRed => candyRed;
        public SolidColorBrush CandyBlue => candyBlue;
    }
}
