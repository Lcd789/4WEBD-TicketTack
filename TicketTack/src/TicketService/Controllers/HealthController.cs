using Microsoft.AspNetCore.Mvc;

namespace TicketTack.TicketService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController :ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("TicketService is running");
        }
    }
}
