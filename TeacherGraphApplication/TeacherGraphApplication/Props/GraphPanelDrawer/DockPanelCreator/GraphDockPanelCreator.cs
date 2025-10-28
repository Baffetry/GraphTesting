using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BoxContainerSpace;

namespace Graph_Panel_Drawer
{
    class GraphDockPanelCreator
    {
        private IContainer container;

        public GraphDockPanelCreator() { container = BoxContainer.Instance(); }

        public DockPanel GetDockPanel(int index, string content)
        {
            var dockPanel = new DockPanel();
            dockPanel.Margin = new Thickness(0, 10, 0, 10);

            var label = new Label
            {
                Content = content
            };

            var textBox = container.GetTextBox(index);

            if (textBox.Parent is Panel panel)
                panel.Children.Remove(textBox);

            dockPanel.Children.Add(label);
            dockPanel.Children.Add(textBox);

            return dockPanel;
        }

    }
}