using Microsoft.AspNetCore.Mvc;

namespace RebbitWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public string Index()
        {
            return "hi";
        }
    }
}
