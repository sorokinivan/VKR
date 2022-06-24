using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VKR.Models.ExamModels;
using VKR.Models.ProfessionalGradeModels;
using VKR.Repositories.Exams;
using VKR.Repositories.ProfessionalGrades;

namespace VKR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly IProfessionalGradeRepository _professionalGradeRepository;
        private readonly IExamRepository _examRepository;

        public ApiController(ILogger<ExamController> logger, IProfessionalGradeRepository professionalGradeRepository, IExamRepository examRepository)
        {
            _professionalGradeRepository = professionalGradeRepository;
            _examRepository = examRepository;
        }

        [HttpPost]
        [Route("CalculationOfProfessionalGrade")]
        public async Task<ExamResultViewData> CalculationOfProfessionalGrade(ProfessionalGradeInput model)
        {
            var result = await _professionalGradeRepository.CalculateProfessionalGrades(model);
            return result;
        }

        [HttpGet]
        [Route("AllGrades")]
        public async Task<List<AllResultsModel>> AllGrades()
        {
            var result = await _examRepository.GetAllResults();
            return result;
        }
    }
}
