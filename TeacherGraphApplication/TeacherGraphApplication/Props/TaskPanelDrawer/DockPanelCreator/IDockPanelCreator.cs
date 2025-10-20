using System.Windows.Controls;

namespace Task_Panel_Drawer
{
    interface IDockPanelCreator
    {
        DockPanel GetDockPanel(string labelContent, int index);
    }
}