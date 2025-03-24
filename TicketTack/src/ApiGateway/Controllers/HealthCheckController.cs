// Dans ApiGateway/Controllers/HealthCheckController.cs
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TicketTack.ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HealthCheckController> _logger;

        public HealthCheckController(IHttpClientFactory httpClientFactory, ILogger<HealthCheckController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet("ping/{serviceName}")]
        public async Task<IActionResult> PingService(string serviceName)
        {
            try
            {
                string serviceUrl = serviceName.ToLower() switch
                {
                    "auth" => "http://authservice/health",
                    "event" => "http://eventservice/health",
                    "ticket" => "http://ticketservice/health",
                    "user" => "http://userservice/health",
                    "notification" => "http://notificationservice/health",
                    _ => throw new ArgumentException($"Unknown service: {serviceName}")
                };

                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(5); // Timeout après 5 secondes

                var response = await client.GetAsync(serviceUrl);

                if (response.IsSuccessStatusCode)
                {
                    return Ok($"200: L'appel au service TicketTack.{serviceName}Service est passé");
                }
                else
                {
                    return StatusCode((int)response.StatusCode,
                        $"L'appel au service TicketTack.{serviceName}Service a échoué avec le code {(int)response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erreur lors de l'appel au service {ServiceName}", serviceName);
                return StatusCode(503, $"Service inaccessible: TicketTack.{serviceName}Service - {ex.Message}");
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Timeout lors de l'appel au service {ServiceName}", serviceName);
                return StatusCode(504, $"Timeout: TicketTack.{serviceName}Service - {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de l'appel au service {ServiceName}", serviceName);
                return StatusCode(500, $"Erreur inattendue: {ex.Message}");
            }
        }
    }
}