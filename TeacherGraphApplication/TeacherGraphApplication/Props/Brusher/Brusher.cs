using System.Windows.Controls;
using System.Windows.Media;

namespace TeacherGraphApplication.Props.Brusher
{
    internal class Brusher
    {
        private SolidColorBrush enableBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4dd49e"));
        private SolidColorBrush disableBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f0f0f0"));
        private SolidColorBrush exitBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#db5174"));

        public SolidColorBrush Enable => enableBrush;
        public SolidColorBrush Disable => disableBrush;
        public SolidColorBrush Exit => exitBrush;
    }
}
