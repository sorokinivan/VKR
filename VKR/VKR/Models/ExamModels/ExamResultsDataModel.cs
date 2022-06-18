namespace VKR.Models.ExamModels
{
    public class ExamResultsDataModel
    {
        public int ExamId { get; set; }
        public List<ExamResultsQuestionsDataModel> Questions { get; set; }
    }

    public class ExamResultsQuestionsDataModel
    {
        public int QuestionId { get; set; }
        public List<int> Answers { get; set; }
    }
}
