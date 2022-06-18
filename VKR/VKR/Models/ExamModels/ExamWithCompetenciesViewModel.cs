namespace VKR.Models.ExamModels
{
    public class ExamWithCompetenciesViewModel
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public List<string> Competencies { get; set; }
    }
}
