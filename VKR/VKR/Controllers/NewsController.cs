using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using VKR.Models.NewsModels;
using VKR.Repositories.News;

namespace VKR.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsRepository _newsRepository;

        public NewsController(ILogger<ExamController> logger, INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetNews([DataSourceRequest] DataSourceRequest request)
        {
            var result = await _newsRepository.GetNewsAsync();
            return Json(result.ToDataSourceResult(request));

        }

        public async Task<IActionResult> NewsDetails(int newsId)
        {
            ViewBag.News = await _newsRepository.GetNewsDetailsAsync(newsId);
            return View("NewsDetails");
        }

        public IActionResult AddNews()
        {
            return View();
        }

        public async Task<IActionResult> SaveNews(NewsDataModel model)
        {
            try
            {
                await _newsRepository.SaveNews(model);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
