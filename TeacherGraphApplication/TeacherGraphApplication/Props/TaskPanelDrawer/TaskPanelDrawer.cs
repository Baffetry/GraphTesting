using System.Windows.Controls;
using TeacherGraphApplication.Props.TaskPanelDrawer.Generators;

namespace Task_Panel_Drawer
{
    class TaskPanelDrawer : ITaskDrawer
    {
        public void Draw(Grid grid)
        {
            var panel = new DockPanelWithScrollViewerGenerator().GenerateDockPanelWithScrollViewer();

            Grid.SetColumn(panel, 1);
            grid.Children.Add(panel);
        }
    }
}