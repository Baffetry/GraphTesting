using System.Windows.Controls;
using System.Windows;

namespace Graph_Panel_Drawer
{
    class StackPanelGenerator : IStackPanelCreator
    {
        private GraphDockPanelCreator generator;
        private int amounOfPanels = (int)Application.Current.FindResource("AmountOfGraphParameters");

        private string[] parameters =
        {
            "Количество вершин:",
            "Количество рёбер:",
        };

        public StackPanelGenerator()
        {
            generator = new GraphDockPanelCreator();
        }

        public StackPanel GetStackPanel()
        {
            var panel = new StackPanel
            {
                Margin = new Thickness(5, 0, 5, 0)
            };

            var label = new Label
            {
                Content = "Параметры графа: ",
                Width = 470,
                Margin = new Thickness(0, 10, 0, 10)
            };

            panel.Children.Add(label);

            for (int i = 0; i < amounOfPanels; i++)
                panel.Children.Add(generator.GetDockPanel(i, parameters[i]));

            return panel;
        }
    }
}