using Graph_Panel_Drawer;
using System.Windows.Controls;
using Task_Panel_Drawer;

class PropertiesDrawer
{
    private ITaskDrawer task;
    private IGraphDrawer graph;

    public PropertiesDrawer(IGraphDrawer graphDrawer, ITaskDrawer taskDrawer)
    {
        task = taskDrawer;
        graph = graphDrawer;
    }

    public void Draw(Grid grid)
    {
        grid.Children.Clear();
        grid.RowDefinitions.Clear();

        graph.Draw(grid);
        task.Draw(grid);
    }
}