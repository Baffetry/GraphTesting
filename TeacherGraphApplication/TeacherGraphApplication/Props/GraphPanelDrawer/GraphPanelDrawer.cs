using System.Windows.Controls;

namespace Graph_Panel_Drawer
{
    class GraphPanelDrawer : IGraphDrawer
    {
        public void Draw(Grid grid)
        {
            var panel = new StackPanelGenerator().GetStackPanel();

            Grid.SetColumn(panel, 0);
            grid.Children.Add(panel);
        }
    }
}