namespace TeacherGraphApplication.CWC
{
    internal class ControlWorkConfigGenerator(List<bool> Flags, List<object> Answers) : IControlWorkConfigGenerator
    {
        private List<bool> flags = Flags;
        private List<object> answers = Answers;

        private static string[] taskContens =
{
            "Посчитайте цикломатическое число.",
            "Посчитайте число независимости.",
            "Посчитайте хроматическое число.",
            "Посчитайте радиус.",
            "Посчитайте диаметр.",
            "Посчитайте число вершинного покрытия.",
            "Посчитайте число реберного покрытия.",
            "Посчитайте плотность графа.",
            "Посчитайте число паросочетания.",
            "Посчитайте хроматический индекс."
        };

        public List<ControlWorkTask> GenerateControlWorkConfig()
        {
            List<ControlWorkTask> cfg = new();

            for (int i = 0; i < flags.Count; i++)
                if (flags[i])
                    cfg.Add(new ControlWorkTask(taskContens[i], true));

            return cfg;
        }
    }
}
