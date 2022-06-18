namespace VKR.Models.ExamModels
{
    public class Exam
    {
        public int Id { get; set; }
        public string ExamName { get; set; }
        public int? Status { get; set; }
        public int? Threshold { get; set; }
    }
}
