using System.Windows.Controls;

namespace Graph_Panel_Drawer
{
    interface IDockPanelCreator
    {
        public DockPanel GetDockPanel(string labelContent);
    }
}