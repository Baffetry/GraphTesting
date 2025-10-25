using System.Windows;
using System.Windows.Controls;

namespace TeacherGraphApplication.Props.TaskPanelDrawer.Generators
{
    internal class DockPanelWithScrollViewerGenerator
    {
        public DockPanel GenerateDockPanelWithScrollViewer()
        {
            var dockPanel = new DockPanel { Margin = new Thickness(100, 0, 0, 50) };

            var label = new LabelGenerator().GenerateLabel("Задания:");
            label.Margin = new Thickness(0, 10, 0, 10);

            DockPanel.SetDock(label, Dock.Top);

            var scrollViewer = new ScrollViewerGenerator().GenerateScrollViewer();

            dockPanel.Children.Add(label);
            dockPanel.Children.Add(scrollViewer);

            return dockPanel;
        }
    }
}
