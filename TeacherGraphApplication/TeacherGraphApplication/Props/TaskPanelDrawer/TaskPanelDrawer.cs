using System.Windows.Controls;
using System.Windows;

namespace Task_Panel_Drawer
{
    class TaskPanelDrawer : ITaskDrawer
    {
        private IMainDockPanel creator;

        public TaskPanelDrawer(IMainDockPanel mainDockPanel)
        {
            creator = mainDockPanel;
        }

        public void Draw(Grid grid)
        {
            var panel = creator.GetDockPanel();

            Grid.SetColumn(panel, 1);
            grid.Children.Add(panel);
        }
    }
}