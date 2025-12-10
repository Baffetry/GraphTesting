using System.Windows;
using System.Windows.Controls;
using TeacherGraphApplication.Props.TaskPanelDrawer.Generators;

class PropertiesDrawer
{
    public void Draw(Grid grid)
    {
        grid.RowDefinitions.Clear();

        var panel = new DockPanelWithScrollViewerGenerator().GenerateDockPanelWithScrollViewer();

        ClearColumn(grid, 1);

        Grid.SetColumn(panel, 1);
        grid.Children.Add(panel);
    }

    private void ClearColumn(Grid grid, int column)
    {
        var itemsToRemove = grid.Children
            .Cast<UIElement>()
            .Where(element => Grid.GetColumn(element) == column)
            .ToList();

        foreach (var item in itemsToRemove)
            grid.Children.Remove(item);
    }
}