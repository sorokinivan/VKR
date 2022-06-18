namespace VKR.Models.ExamModels
{
    public class ExamCreationDataModel
    {
        public string ExamName { get; set; }
        public int CompetencyId { get; set; }
        public int Threshold { get; set; }
        public List<ExamCreationQuestionsDataModel> Questions { get; set; }
    }

    public class ExamCreationQuestionsDataModel
    {
        public string QuestionText { get; set; }
        public int QuestionIndicator { get; set; }
        public List<ExamCreationAnswersDataModel> Answers { get; set; }
    }

    public class ExamCreationAnswersDataModel
    {
        public string AnswerText { get; set; }
        public bool AnswerIsCorrect { get; set; }
    }
}
