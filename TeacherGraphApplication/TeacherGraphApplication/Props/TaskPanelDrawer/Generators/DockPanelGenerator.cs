using System.Windows;
using System.Windows.Controls;
using TeacherGraphApplication.Props.TaskPanelDrawer.Generators.Container;

namespace TeacherGraphApplication.Props.TaskPanelDrawer.Generators
{
    internal class DockPanelGenerator
    {
        private BoxContainer container;
        private LabelGenerator labelGenerator;

        public DockPanelGenerator(LabelGenerator labelGenerator)
        {
            container = BoxContainer.Instance();
            this.labelGenerator = labelGenerator;
        }

        public DockPanel GenerateDockPanel(int index, string content)
        {
            var dockPanel = new DockPanel();
            dockPanel.Margin = new Thickness(0, 10, 0, 10);

            var checkBox = container[index];
            var label = labelGenerator.GenerateLabel(content);

            DockPanel.SetDock(label, Dock.Left);

            dockPanel.Children.Add(label);

            if (checkBox.Parent is Panel panel)
                panel.Children.Remove(checkBox);

            dockPanel.Children.Add(checkBox);

            return dockPanel;
        }
    }
}
