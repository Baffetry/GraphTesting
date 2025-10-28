using System.Windows.Controls;

namespace TeacherGraphApplication.Props.TaskPanelDrawer.Generators
{
    internal class LabelGenerator
    {
        public Label GenerateLabel(string content)
        {
            return new Label { Content = content, Width = 750 };
        }
    }
}
