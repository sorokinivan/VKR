namespace VKR.Models.ExamModels
{
    public class ExamViewModel
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public List<QuestionsViewModel> Questions { get; set; }
    }

    public class QuestionsViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public List<AnswersViewModel> Answers { get; set; }
    }

    public class AnswersViewModel
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; }
        public bool AnswerIsCorrect { get; set; } //TODO УДАЛИТЬ
    }
}
