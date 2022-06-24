using Microsoft.AspNetCore.Mvc;

namespace VKR.Controllers
{
    public class InfoController : Controller
    {
        public IActionResult Faq()
        {
            return View();
        }
    }
}
