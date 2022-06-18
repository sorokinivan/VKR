namespace VKR.Models.ProfessionalGradeModels
{
    public class ProfessionalGrade
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string UserId { get; set; }
        public int CompetencyId { get; set; }
        public double Grade { get; set; }
        public string StringGrade { get; set; }
    }
}
