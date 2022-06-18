using VKR.Models;
using VKR.Models.ProfessionalGradeModels;

namespace VKR.Repositories.ProfessionalGrades
{
    public interface IProfessionalGradeRepository
    {
        Task<ExamResultViewData> CalculateProfessionalGrades(ProfessionalGradeInput professionalGradeInputs);
        Task SetProfessionalGrades(IEnumerable<ExamResult> professionalGrades);
        Task<IEnumerable<ExamResultViewData>> GetProfessionalGradeByExamAndUser(string userId, int examId);
        //Task<List<double>> CalculateIndicatorScore();
        //Task<List<ProfessionalGrade>> GetProfessionalGrades();
        //Task<List<ProfessionalGrade>> GetProfessionalGradesByExaminer(int examinerId);
        //Task<List<ProfessionalGrade>> GetProfessionalGradesByExam(int examId);
        //Task<List<ProfessionalGrade>> GetProfessionalGradesByGradeDate(DateTime gradeDate);
        //Task<ProfessionalGrade> GetProfessionalGrade(int professionalGradeId);
    }
}
