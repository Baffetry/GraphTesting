using System.Collections;

namespace TeacherGraphApplication.Props.TaskPanelDrawer.Generators.Container
{
    internal interface IContainer : IContainerSerializer, IEnumerable
    {
        void GetPanelsWithCheckBox();
        bool IsChanged();
    }
}
