using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VKR.Models;
using VKR.Repositories.ProfessionalGrades;

namespace VKR.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProfessionalGradeRepository _professionalGradeRepository;

        public HomeController(ILogger<HomeController> logger, IProfessionalGradeRepository professionalGradeRepository)
        {
            _logger = logger;
            _professionalGradeRepository = professionalGradeRepository;
        }

        //[Authorize]
        public async Task<IActionResult> Index()
        {
            ViewBag.User = User.Identity.Name;
            //var t = await _professionalGradeRepository.GetProfessionalGrades();
            return View();
        }

        public async Task<IActionResult> ExamMain()
        {
            //var t = await _professionalGradeRepository.GetProfessionalGrades();
            return View("ExamMain");
        }
    }
}