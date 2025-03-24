using Microsoft.AspNetCore.Mvc;

namespace TicketTack.NotificationService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("NotificationService is running");
        }
    }
}
