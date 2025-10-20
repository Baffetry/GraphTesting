using Graph_Panel_Drawer;
using System.Windows;
using System.Windows.Controls;
using Task_Panel_Drawer;

class PropertiesDrawer : IDrawProperties
{
    private static IDrawProperties? _propertiesDrower; // Singelton pattern
    private ITaskDrawer _taskDrower;
    private IGraphDrawer _graphDrawer;
    private IObserve _observer;

    private PropertiesDrawer(IGraphDrawer graphDrawer, ITaskDrawer taskDrower)
    {
        _graphDrawer = graphDrawer;
        _taskDrower = taskDrower;
        _observer = ObserveCheckBox.GetObserver();
    }

    // Singelton pattern 
    public static PropertiesDrawer SetPropertiesDrower(IGraphDrawer graphDrawer, ITaskDrawer taskDrower)
    {
        if (_propertiesDrower is null)
            _propertiesDrower = new PropertiesDrawer(graphDrawer, taskDrower);

        return (PropertiesDrawer)_propertiesDrower;
    }

    public void DrowProperties(Grid grid)
    {
        grid.Children.Clear();
        grid.RowDefinitions.Clear();

        _graphDrawer.Draw(grid);
        _taskDrower.Draw(grid);
    }
}