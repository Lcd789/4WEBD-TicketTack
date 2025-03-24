using Microsoft.AspNetCore.Mvc;

namespace TicketTack.EventService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() {
            return Ok("EventService is running");
        }
    }
}
