using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using VKR.Repositories.Exams;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using VKR.Models;
using VKR.Repositories.ProfessionalGrades;
using VKR.Models.ExamModels;

namespace VKR.Controllers
{
    public class ExamController : Controller
    {
        private readonly IExamRepository _examRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IProfessionalGradeRepository _professionalGradeRepository;

        public ExamController(ILogger<ExamController> logger, IExamRepository examRepository, UserManager<IdentityUser> userManager, IProfessionalGradeRepository professionalGradeRepository)
        {
            _examRepository = examRepository;
            _userManager = userManager;
            _professionalGradeRepository = professionalGradeRepository;
        }

        //[Authorize]
        public async Task<IActionResult> FinishedExams()
        {
            ViewBag.Test = 1;
            //var t = await _professionalGradeRepository.GetProfessionalGrades();
            ViewBag.CompetenciesCount = 2;
            ViewBag.Names = new string[] {"Компетенция 1", "Компетенция 2"};
            return View("FinishedExams");
        }

        public async Task<IActionResult> ExamsCreator()
        {
            ViewBag.Competencies = await _examRepository.GetCompetencies();
            return View();
        }

        public IActionResult ExamQuestionsCreator()
        {
            return View();
        }

        public IActionResult CompetenciesCreator()
        {
            return View();
        }

        public IActionResult IndicatorsCreator()
        {
            return View();
        }

        public async Task<IActionResult> CompetenciesIndicatorsCreator()
        {
            ViewBag.Competencies = await _examRepository.GetCompetencies();
            ViewBag.Indicators = await _examRepository.GetIndicators();
            return View();
        }

        public async Task<IActionResult> GetIndicatorsByCompetency(int competencyId)
        {
            if(competencyId == 0)
            {
                competencyId = 1;
            }
            var result = await _examRepository.GetIndicatorsByCompetency(competencyId);
            return Json(new { success = true, data = result });
        }

        [HttpPost]
        public async Task<IActionResult> SaveExam(ExamCreationDataModel model)
        {
            try
            {
                await _examRepository.SaveExam(model);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        public IActionResult ChangeExamsStatuses()
        {
            return View();
        }

        public async Task<IActionResult> GetExams([DataSourceRequest] DataSourceRequest request)
        {
            var result = await _examRepository.GetExams();
            return Json(result.ToDataSourceResult(request));

        }

        public async Task<IActionResult> OpenExam(int examId)
        {
            try
            {
                await _examRepository.OpenExam(examId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        public async Task<IActionResult> CloseExam(int examId)
        {
            try
            {
                await _examRepository.CloseExam(examId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveExamResults(ExamResultsDataModel model)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                await _examRepository.SaveExamResults(model, userId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompetencyIndicators([FromBody]CompetencyIndicatorsModel model)
        {
            try
            {
                await _examRepository.CreateCompetencyIndicators(model);
                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Internal error" });
            }
        }

        public async Task<IActionResult> CreateNewCompetency(string name)
        {
            try
            {
                await _examRepository.CreateNewCompetency(name);
                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Internal error" });
            }
        }

        public async Task<IActionResult> CreateNewIndicator(string name)
        {
            try
            {
                await _examRepository.CreateNewIndicator(name);
                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Internal error" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExamGradesPartial(int examId)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var result = await _professionalGradeRepository.GetProfessionalGradeByExamAndUser(userId, examId);
                var grades = result.Select(x => x.Grades).FirstOrDefault();
                ViewBag.GradesCount = grades.Count();
                ViewBag.GradesNames = grades.Select(x => x.CompetencyName).ToList();
                ViewBag.TestData = result;
                return PartialView();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public async Task<IActionResult> GetAvailableExams()
        //{
        //    try
        //    {
        //        var userId = _userManager.GetUserId(User);
        //        var result = await _examRepository.GetAvailableExamsPerCurrentUser(userId);
        //        return Json(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex);
        //    }
        //}

        //[HttpPost]
        public async Task<IActionResult> GetFinishedExams([DataSourceRequest] DataSourceRequest request)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _examRepository.GetFinishedExamsPerCurrentUser(userId);
            return Json(result.ToDataSourceResult(request));
        }

        public IActionResult GetExamResults()
        {
            return View("ExamResults");
        }

        public async Task<IActionResult> GetExamPage(int examId)
        {
            var model = await _examRepository.GetExamById(examId); 
            return View("Exam", model);
        }

        public IActionResult AvailableExams()
        {
            return View();
        }

        public async Task<IActionResult> GetAvailableExams([DataSourceRequest] DataSourceRequest request)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _examRepository.GetAvailableExamsPerCurrentUser(userId);
            return Json(result.ToDataSourceResult(request));
        }

        public async Task<IActionResult> GetResultsData([DataSourceRequest] DataSourceRequest request, int examId)
        {
            if (examId == 0)
            { examId = 1; }
                var userId = _userManager.GetUserId(User);
                var result = await _professionalGradeRepository.GetProfessionalGradeByExamAndUser(userId, examId);
            
                return Json(result.ToDataSourceResult(request));
            return Json(null);
        }
    }
}
