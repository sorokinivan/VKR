namespace VKR.Models.ProfessionalGradeModels
{
    public class ProfessionalGradeInput
    {
        //public int CompetenceId { get; set; }
        public int ExamId { get; set; }
        public int? Threshold { get; set; }
        public List<ProfessionalGradeInputCompetency> Competencies { get; set; }
    }
}
