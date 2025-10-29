using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudentGraphApplication
{
    public class StudentResults
    {
        [JsonPropertyName("student")]
        [JsonInclude]
        public Student Student { get; set; }

        [JsonPropertyName("task_answers")]
        [JsonInclude]
        public List<TaskAnswer> TaskAnswers { get; set; }

        public StudentResults(Student student)
        {
            Student = student;
            TaskAnswers = new List<TaskAnswer>();
        }

        public StudentResults(Student student, List<TaskAnswer> taskAnswers)
        {
            Student = student;
            TaskAnswers = taskAnswers;
        }

        public void AddAnswer(string question, object answer)
        {
            TaskAnswers.Add(new TaskAnswer(question, answer));
        }

        public object GetAnswer(int taskIndex)
        {
            if (taskIndex >= 0 && taskIndex < TaskAnswers.Count)
                return TaskAnswers[taskIndex].Answer;
            return null;
        }

        public List<object> GetAllAnswers()
        {
            var answers = new List<object>();
            foreach (var taskAnswer in TaskAnswers)
            {
                answers.Add(taskAnswer.Answer);
            }
            return answers;
        }
        public void AddOrUpdateAnswer(string question, object answer)
        {
            var existingAnswer = TaskAnswers.FirstOrDefault(ta => ta.Question == question);

            if (existingAnswer != null)
            {
                existingAnswer.Answer = answer;
            }
            else
            {
                TaskAnswers.Add(new TaskAnswer(question, answer));
            }
        }

        
        public bool HasAnswerForQuestion(string question)
        {
            return TaskAnswers.Any(ta => ta.Question == question);
        }

        public object GetAnswerForQuestion(string question)
        {
            var taskAnswer = TaskAnswers.FirstOrDefault(ta => ta.Question == question);
            return taskAnswer?.Answer;
        }

        public object GetAnswerByIndex(int index)
        {
            if (index >= 0 && index < TaskAnswers.Count)
                return TaskAnswers[index].Answer;
            return null;
        }
    }
}
