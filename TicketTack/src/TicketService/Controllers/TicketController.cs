// Dans TicketService/Controllers/TicketController.cs
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TicketTack.Shared.DTOs;
using TicketTack.Shared.Infrastructure.MongoDB;
using TicketTack.Shared.Infrastructure.RabbitMQ;
using TicketTack.Shared.Models;

namespace TicketTack.TicketService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly IMongoRepository<Ticket> _ticketRepository;
        private readonly IMongoRepository<Event> _eventRepository;
        private readonly IMongoRepository<User> _userRepository;
        private readonly Producer _producer;
        private readonly ILogger<TicketController> _logger;

        public TicketController(
            IMongoRepository<Ticket> ticketRepository,
            IMongoRepository<Event> eventRepository,
            IMongoRepository<User> userRepository,
            Producer producer,
            ILogger<TicketController> logger)
        {
            _ticketRepository = ticketRepository;
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _producer = producer;
            _logger = logger;
        }

        [HttpPost("purchase")]
        [Authorize]
        public async Task<ActionResult<TicketDto>> PurchaseTicket(PurchaseTicketDto purchaseDto)
        {
            try
            {
                // Récupérer l'événement
                var @event = await _eventRepository.GetByIdAsync(purchaseDto.EventId);
                if (@event == null)
                {
                    return NotFound("Event not found");
                }

                // Vérifier la disponibilité des billets
                if (@event.TicketsAvailable <= 0)
                {
                    return BadRequest("No tickets available for this event");
                }

                // Récupérer l'utilisateur
                var userId = User.FindFirst("UserId")?.Value;
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return BadRequest("User not found");
                }

                // Créer un ticket unique
                var ticketNumber = Guid.NewGuid().ToString("N");
                var ticket = new Ticket
                {
                    EventId = purchaseDto.EventId,
                    UserId = userId,
                    TicketNumber = ticketNumber,
                    PurchasePrice = @event.Price,
                    PurchaseDate = DateTime.UtcNow,
                    QRCodeData = $"{@event.Id}:{userId}:{ticketNumber}"
                };

                // Simuler le paiement par carte bancaire
                // Dans un vrai système, vous devriez intégrer un processeur de paiement
                bool paymentSuccessful = await SimulatePayment(purchaseDto);
                if (!paymentSuccessful)
                {
                    return BadRequest("Payment failed");
                }

                // Enregistrer le ticket
                await _ticketRepository.AddAsync(ticket);

                // Mettre à jour l'événement (décrémenter le nombre de billets disponibles)
                @event.TicketsAvailable--;
                await _eventRepository.UpdateAsync(@event);

                // Mettre à jour l'utilisateur (ajouter le ticket à sa liste)
                user.PurchasedTickets.Add(ticket.Id);
                await _userRepository.UpdateAsync(user);

                // Envoyer une notification via RabbitMQ
                var notificationMessage = new
                {
                    Type = "TicketPurchased",
                    UserId = userId,
                    UserEmail = user.Email,
                    UserPhone = user.PhoneNumber,
                    EventName = @event.Name,
                    EventDate = @event.StartDate,
                    TicketNumber = ticketNumber,
                    PurchaseDate = ticket.PurchaseDate
                };

                _producer.PublishMessageAsync("notification_exchange", "ticket_purchased", notificationMessage);

                // Retourner le DTO
                var ticketDto = new TicketDto
                {
                    Id = ticket.Id,
                    EventId = ticket.EventId,
                    TicketNumber = ticket.TicketNumber,
                    Price = ticket.PurchasePrice,
                    PurchaseDate = ticket.PurchaseDate,
                    QRCodeData = ticket.QRCodeData
                };

                return CreatedAtAction("GetById", new { id = ticket.Id }, ticketDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error purchasing ticket");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<TicketDto>> GetById(string id)
        {
            try
            {
                var ticket = await _ticketRepository.GetByIdAsync(id);

                if (ticket == null)
                    return NotFound();

                // Vérifier que l'utilisateur est le propriétaire du ticket ou un admin
                var userId = User.FindFirst("UserId")?.Value;
                var userRole = User.FindFirst("Role")?.Value;

                if (ticket.UserId != userId && userRole != "Admin")
                    return Forbid();

                var ticketDto = new TicketDto
                {
                    Id = ticket.Id,
                    EventId = ticket.EventId,
                    TicketNumber = ticket.TicketNumber,
                    Price = ticket.PurchasePrice,
                    PurchaseDate = ticket.PurchaseDate,
                    QRCodeData = ticket.QRCodeData
                };

                return Ok(ticketDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ticket with id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        private Task<bool> SimulatePayment(PurchaseTicketDto purchaseDto)
        {
            // Simuler un processus de paiement
            // Dans un système réel, vous intégreriez un service de paiement comme Stripe
            return Task.FromResult(true);
        }
    }
}