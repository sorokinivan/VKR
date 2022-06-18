namespace VKR.Models.ExamModels
{
    public class Question
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int IndicatorId { get; set; }
        public string QuestionText { get; set; }
    }
}
