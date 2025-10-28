using System.Windows.Controls;
using System.IO;
using System.Text.Json;
using TeacherGraphApplication.Props.TaskPanelDrawer.Generators;
using TeacherGraphApplication.Props.GraphPanelDrawer.DockPanelCreator;
using System.Windows;

namespace BoxContainerSpace
{
    public class BoxContainer : IContainer
    {
        #region Fields
        private const string _statesPath = "boxesStates.json";
        private const string _textPath = "boxesText.json";

        private int amountOfTask = (int)Application.Current.FindResource("AmountOfTask");
        private int amountOfGraphParams = (int)Application.Current.FindResource("AmountOfGraphParameters");

        private static BoxContainer? instance; // pattern singelton implementation

        private List<CheckBox> boxes;
        private List<bool> states;

        private List<TextBox> textBoxes;
        private List<string> text;
        #endregion

        #region Constructor
        private BoxContainer() 
        {
            boxes = new List<CheckBox>();
            states = new List<bool>();

            textBoxes = new List<TextBox>();
            text = new List<string>();
            
            GetPanelsWithCheckBox();
        }
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
            for (int i = 0; i < amountOfTask; i++)
                boxes[i].IsChecked = states[i];
        }

        private void SetTextToTextBoxes()
        {
            for (int i = 0; i < amountOfGraphParams; i++)
                textBoxes[i].Text = text[i];
        }

        private void UpdateStates()
        {
            for (int i = 0; i < amountOfTask; i++)
                states[i] = (bool)boxes[i].IsChecked;
        }

        private void UpdateText()
        {
            for (int i = 0; i < amountOfGraphParams; i++)
                text[i] = textBoxes[i].Text;
        }

        private void SetDefault()
        {
            for (int i = 0; i < amountOfTask; i++)
            {
                boxes[i].IsChecked = false;
                states[i] = false;
            }

            for (int i = 0; i < textBoxes.Count; i++)
            {
                textBoxes[i].Text = string.Empty;
                text[i] = string.Empty;
            }
        }
        #endregion

        #region IContainer implementatino
        public void GetPanelsWithCheckBox()
        {
            CheckBoxGenerator generator = new CheckBoxGenerator();
            TextBoxGenerator textBoxGenerator = new TextBoxGenerator();

            for (int i = 0; i < amountOfTask; i++)
            {
                boxes.Add(generator.GenerateCheckBox(i));
                states.Add(false);
            }

            for (int i = 0; i < amountOfGraphParams; i++)
            {
                textBoxes.Add(textBoxGenerator.GenerateTextBox(i));
                text.Add(string.Empty);
            }
        }

        public void Update()
        {
            UpdateStates();
            UpdateText();
        }

        public bool IsChanged()
        {
            Update();

            foreach (var state in states)
                if (state)
                    return true;

            foreach (var txt in text)
                if (!txt.Equals(string.Empty))
                    return true;

            return false;
        }

        public TextBox GetTextBox(int idx)
        {
            return textBoxes[idx];
        }

        public CheckBox GetCheckBox(int idx)
        {
            return boxes[idx];
        }

        public List<bool> GetStates()
        {
            return states;
        }
        #endregion

        #region IContainerSerializer implementatiton
        public void Stop()
        {
            File.Delete(_statesPath);
            File.Delete(_textPath);
            SetDefault();
        }

        public void Save()
        {
            //var stateSerializer = new Serializer<List<bool>>();
            //var textSerializer = new Serializer<List<string>>();


            using (File.Create(_statesPath)) { } // Перезапись файла с нуля
            using (File.Create(_textPath)) { } // Перезапись файла с нуля

            UpdateStates(); // Обновление состояний в соответствии с checkbox

            var stateJson = JsonSerializer.Serialize(states);
            File.WriteAllText(_statesPath, stateJson);

            UpdateText();

            var textJson = JsonSerializer.Serialize(text);
            File.WriteAllText(_textPath, textJson);

            SetDefault(); // Отчистка состояний, для последующей загрузки
        }

        public void Load() // Десериализация состояний
        {
            if (File.Exists(_statesPath))
            {
                var json = File.ReadAllText(_statesPath);
                states = JsonSerializer.Deserialize<List<bool>>(json);
            }

            if (File.Exists(_textPath))
            {
                var json = File.ReadAllText(_textPath);
                text = JsonSerializer.Deserialize<List<string>>(json);
            }

            SetStatesToCheckBoxes();
            SetTextToTextBoxes();
        }
        #endregion
    }
}