
using System.Windows.Controls;

internal interface IObserve
{
    void Add(CheckBox box);
    void Update(CheckBox box);
    int FindBox(CheckBox box);
    bool IsChecked(int idx);
}