namespace VKR.Models.ProfessionalGradeModels
{
    public class ProfessionalGradeInputCompetency
    {
        public int Id { get; set; }
        public string CompetencyName { get; set; }
        public List<ProfessionalGradeInputIndicator> Indicators { get; set; }
    }
}
