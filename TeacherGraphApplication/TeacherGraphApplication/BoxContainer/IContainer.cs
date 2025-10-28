using System.Windows.Controls;

namespace BoxContainerSpace
{
    internal interface IContainer : IContainerSerializer
    {
        void GetPanelsWithCheckBox();
        void Update();
        bool IsChanged();
        TextBox GetTextBox(int idx);
        CheckBox GetCheckBox(int idx);
        List<bool> GetStates();
    }
}
