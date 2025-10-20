using System.Windows.Controls;
using System.Windows;

namespace Graph_Panel_Drawer
{
    class StackPanelCreator : IStackPanelCreator
    {
        private IDockPanelCreator creator;

        private string[] parameters =
        {
            "Количество вершин:",
            "Количество рёбер:"
        };

        public StackPanelCreator(IDockPanelCreator dockPanelCreator)
        {
            creator = dockPanelCreator;
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

            foreach (var parameter in parameters)
                panel.Children.Add(creator.GetDockPanel(parameter));

            return panel;
        }
    }
}