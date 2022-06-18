using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VKR.Core;
using VKR.Models.RequestModels;
using VKR.Repositories.Requests;

namespace VKR.Controllers
{
    public class RequestController : Controller
    {
        private readonly IRequestRepository _requestRepository;
        private readonly UserManager<IdentityUser> _userManager;
        public RequestController(IRequestRepository requestRepository, UserManager<IdentityUser> userManager)
        {
            _requestRepository = requestRepository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetUserRequests([DataSourceRequest] DataSourceRequest request)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _requestRepository.GetRequestsByUser(userId);
            return Json(result.ToDataSourceResult(request));
        }

        [HttpPost]
        public async Task<IActionResult> SaveResultRequest(RequestResultModel model)
        {
            try
            {
                await _requestRepository.SaveResultRequest(model);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult NewRequestPartial()
        {
            return PartialView("NewRequestPartial");
        }

        [HttpPost]
        public IActionResult NotCheckedRequestPartial(int examId)
        {
            ViewBag.User = _userManager.GetUserId(User);
            ViewBag.Exam = examId;
            return PartialView("NotCheckedRequestPartial");
        }

        public async Task<IActionResult> GetNotCheckedRequests([DataSourceRequest] DataSourceRequest request)
        {
            var result = await _requestRepository.GetNotCheckedRequests();
            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult NotCheckedRequests()
        {
            return View("NotCheckedRequests");
        }

        public async Task<IActionResult> GetExamsForRequest([DataSourceRequest] DataSourceRequest request)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _requestRepository.GetExamsForRequest(userId);
            return Json(result.ToDataSourceResult(request));
        }

        [HttpPost]
        public async Task<IActionResult> SaveRequest(int examId)
        {
            var userId = _userManager.GetUserId(User);
            var model = new Request {
                ExamId = examId,
                Status = (int)RequestStatusEnum.NotChecked,
                RequestText = null,
                UserId = userId 
            };
            await _requestRepository.SaveRequest(model);
            return Json(new { success = true });
        }
    }
}
