using System.Windows.Controls;

namespace Graph_Panel_Drawer
{
    class GraphPanelDrawer : IGraphDrawer
    {
        private IStackPanelCreator creator;

        public GraphPanelDrawer(IStackPanelCreator stackPanelCreator)
        {
            creator = stackPanelCreator;
        }

        public void Draw(Grid grid)
        {
            var panel = creator.GetStackPanel();

            Grid.SetColumn(panel, 0);
            grid.Children.Add(panel);
        }
    }
}