namespace TeacherGraphApplication.Props.TaskPanelDrawer.Generators.Container
{
    internal interface IContainerSerializer
    {
        void Stop();
        void Save(string path = null);
        void Load(string path = null);
    }
}
