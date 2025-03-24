using Microsoft.AspNetCore.Mvc;

namespace TicketTack.UserService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("UserService is running");
        }
    }
}
