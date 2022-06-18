using VKR.Models.ExamModels;
using VKR.Models.RequestModels;

namespace VKR.Repositories.Requests
{
    public interface IRequestRepository
    {
        Task SaveRequest(Request model);
        Task SaveResultRequest(RequestResultModel model);
        Task<List<RequestViewModel>> GetNotCheckedRequests();
        Task<List<RequestViewModel>> GetRequestsByUser(string user);
        Task<List<ExamWithCompetenciesViewModel>> GetExamsForRequest(string user);
    }
}
