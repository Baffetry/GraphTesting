using System.Windows;
using System.Windows.Controls;

namespace TeacherGraphApplication.Props.TaskPanelDrawer.Generators
{
    internal class ScrollViewerGenerator
    {
        private DockPanelGenerator generator;
        private int amountOfPanels = (int)Application.Current.FindResource("AmountOfTask");

        private string[] taskName =
        {
            "Посчитать цикломатическое число",
            "Посчитать число независимости",
            "Посчитать хроматическое число",
            "Посчитать радиус",
            "Посчитать диаметр",
            "Посчитать число вершинного покрытия",
            "Посчитать число реберного покрытия",
            "Посчитать плотность графа",
            "Посчитать число паросочетания",
            "Посчитать хроматический индекс"
        };

        public ScrollViewerGenerator()
        {
            generator = new DockPanelGenerator(new LabelGenerator());
        }

        public ScrollViewer GenerateScrollViewer()
        {
            var scrollViewer = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
            var tasksGrid = new Grid { Name = "TasksGrid" };

            SetPanelInGrid(tasksGrid);

            scrollViewer.Content = tasksGrid;

            return scrollViewer;
        }

        private void SetPanelInGrid(Grid grid)
        {
            for (int i = 0; i < amountOfPanels; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var panel = generator.GenerateDockPanel(i, taskName[i]);

                Grid.SetRow(panel, i);
                grid.Children.Add(panel);
            }
        }
    }
}
