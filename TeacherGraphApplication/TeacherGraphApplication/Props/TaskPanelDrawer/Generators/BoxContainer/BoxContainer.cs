using System.Collections;
using System.Windows.Controls;
using Serializer;
using System.IO;
using System.Text.Json;

namespace TeacherGraphApplication.Props.TaskPanelDrawer.Generators.Container
{
    public class BoxContainer : IContainer
    {
        #region Fields
        private const string _path = "boxesStates.json";

        private static BoxContainer instance; // pattern singelton implementation

        private int count;

        private List<DockPanel> parents;
        private List<CheckBox> boxes;
        private List<bool> states;
        #endregion

        #region Constructor
        private BoxContainer() 
        {
            count = new Serializer<int>().LoadFromFile("AmountOfTasks.json");
            
            parents = new List<DockPanel>();
            boxes = new List<CheckBox>();
            states = new List<bool>();
            
            GetPanelsWithCheckBox();
        }
        #endregion

        #region Properties
        public CheckBox this[int index] 
            => boxes[index];
        #endregion

        #region Functional methods of the class
        public static BoxContainer Instance() // pattern singelton implementation
        {
            if (instance is null)
                instance = new BoxContainer();
            return instance;
        }

        private void SetStatesToCheckBoxes() // Установка состояний, после их десериализации
        {
            for (int i = 0; i < count; i++)
                boxes[i].IsChecked = states[i];
        }

        private void UpdateStates()
        {
            for (int i = 0; i < count; i++)
                states[i] = (bool)boxes[i].IsChecked;
        }

        private void SetDefaultStates()
        {
            for (int i = 0; i < count; i++)
            {
                boxes[i].IsChecked = false;
                states[i] = false;
            }
        }
        #endregion

        #region IContainer implementatino
        public void GetPanelsWithCheckBox()
        {
            CheckBoxGenerator generator = new CheckBoxGenerator();
            for (int i = 0; i < count; i++)
            {
                boxes.Add(generator.GenerateCheckBox(i));
                states.Add(false);
            }
        }

        public bool IsChanged()
        {
            UpdateStates();

            foreach (var state in states)
                if (state)
                    return true;
            return false;
        }
        #endregion

        #region IContainerSerializer implementatiton
        public void Stop()
        {
            File.Delete(_path);
        }

        public void Save(string path = null)
        {
            path = path ?? _path;

            var serializer = new Serializer<List<bool>>();

            using (File.Create(path)) { } // Перезапись файла с нуля

            UpdateStates(); // Обновление состояний в соответствии с checkbox
            serializer.SaveToFile(path, states); // Сериализация состояний в файл
            SetDefaultStates(); // Отчистка состояний, для последующей загрузки
        }

        public void Load(string path = null) // Десериализация состояний
        {
            path = path ?? _path;

            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                states = JsonSerializer.Deserialize<List<bool>>(json);
            }

            SetStatesToCheckBoxes();
        }
        #endregion

        #region IEnumerable implementation
        public IEnumerator<CheckBox> GetEnumerator()
        {
            foreach (var box in boxes)
                yield return box;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}