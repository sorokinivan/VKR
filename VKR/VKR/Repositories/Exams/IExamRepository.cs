using VKR.Models.ExamModels;

namespace VKR.Repositories.Exams
{
    public interface IExamRepository
    {
        Task<IEnumerable<Exam>> GetAvailableExamsPerCurrentUser(string userId);
        Task<IEnumerable<Exam>> GetFinishedExamsPerCurrentUser(string userId);
        Task CreateNewIndicator(string indicatorName);
        Task CreateNewCompetency(string competencyName);
        Task<List<Competency>> GetCompetencies();
        Task<List<Indicator>> GetIndicators();
        Task OpenExam(int examId);
        Task CloseExam(int examId);
        Task CreateCompetencyIndicators(CompetencyIndicatorsModel model);
        Task<List<Indicator>> GetIndicatorsByCompetency(int CompetencyId);
        Task SaveExam(ExamCreationDataModel model);
        Task<ExamViewModel> GetExamById(int examId);
        Task SaveExamResults(ExamResultsDataModel model, string userId);
        Task<List<Exam>> GetExams();
        Task <List<AllResultsModel>> GetAllResults();
    }
}
