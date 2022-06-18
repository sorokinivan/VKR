using VKR.Models.ExamModels;

namespace VKR.Models.ProfessionalGradeModels
{
    public class ExamResult
    {

        public int Id { get; set; }
        public string UserId { get; set; }
        public int ExamId { get; set; }

        public double R { get; set; }
        
        public double H { get; set; }
        
        public double D { get; set; }
        

        public double O { get; set; }
        

    }
}
