using Microsoft.AspNetCore.Mvc;

namespace TicketTack.AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("AuthService is running");
        }
    }
}
