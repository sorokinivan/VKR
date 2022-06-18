using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VKR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiTestController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(1);
        }
    }
}
