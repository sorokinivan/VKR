namespace VKR.Models.ExamModels
{
    public class ExamDichotomousGrades
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ExamId { get; set; }
        public int QuestionId { get; set; }
        public int Grade { get; set; }
    }
}
