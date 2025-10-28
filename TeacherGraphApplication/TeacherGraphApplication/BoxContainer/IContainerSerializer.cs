namespace BoxContainerSpace
{
    internal interface IContainerSerializer
    {
        void Stop();
        void Save();
        void Load();
    }
}
