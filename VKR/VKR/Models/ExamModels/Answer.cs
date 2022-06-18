namespace VKR.Models.ExamModels
{
    public class Answer
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public bool IsCorrect { get; set; }
        public string AnswerText { get; set; }
    }
}
