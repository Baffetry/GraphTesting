using System.Windows;
using System.Windows.Controls;

namespace Task_Panel_Drawer
{
    internal class MainDockPanel : IMainDockPanel
    {
        private IDockPanelCreator creator;

        private string[] _taskName =
        {
            "Посчитать цикломатическое число",
            "Посчитать число независимости",
            "Посчитать хроматическое число",
            "Посчитать радиус", "диаметр",
            "Посчитать число вершинного покрытия",
            "Посчитать число реберного покрытия",
            "Посчитать плотность графа",
            "Посчитать число паросочетания",
            "Посчитать хроматический индекс"
        };

        public MainDockPanel(IDockPanelCreator dockPanelCreator)
        {
            creator = dockPanelCreator;
        }

        public DockPanel GetDockPanel()
        {
            var panel = new DockPanel
            {
                Margin = new Thickness(100, 0, 0, 200),
            };

            var label = new Label
            {
                Content = "Задания: ",
                Margin = new Thickness(0, 10, 0, 10)
            };

            DockPanel.SetDock(label, Dock.Top);

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            };

            var tasksGrid = new Grid
            {
                Name = "TasksGrid"
            };

            SetGrid(tasksGrid);

            scrollViewer.Content = tasksGrid;

            panel.Children.Add(label);
            panel.Children.Add(scrollViewer);

            return panel;
        }

        private void SetGrid(Grid grid)
        {
            for (int i = 0; i < _taskName.Length; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition
                {
                    Height = GridLength.Auto
                });

                var panel = creator.GetDockPanel(_taskName[i], i);

                Grid.SetRow(panel, i);
                grid.Children.Add(panel);

            }
        }
    }
}