
using System.Windows.Controls;

internal class ObserveCheckBox : IObserve
{

    #region Singelton pattern
    private static ObserveCheckBox observe;
    List<CheckBox> checkedList;
    List<bool> obs;

    private ObserveCheckBox()
    {
        checkedList = new List<CheckBox>();
        obs = new List<bool>();
    }

    public static ObserveCheckBox GetObserver()
    {
        if (observe is null)
            observe = new ObserveCheckBox();
        return observe;
    }

    #endregion

    #region IObserve
    public void Add(CheckBox box)
    {
        checkedList.Add(box);
        obs.Add(false);
    }

    public int FindBox(CheckBox box)
    {
        for (int i = 0; i < checkedList.Count; i++)
            if (checkedList[i].Name == box.Name)
                return i;
        return -1;
    }

    public bool IsChecked(int idx)
    {
        return obs[idx];
    }

    public void Update(CheckBox box)
    {
        for (int i = 0; i < checkedList.Count; i++)
        {
            if (checkedList[i].Name == box.Name)
            {
                obs[i] = (bool)box.IsChecked;
                break;
            }
        }
    }
    #endregion
}