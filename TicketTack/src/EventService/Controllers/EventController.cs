using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TicketTack.Shared.DTOs;
using TicketTack.Shared.Infrastructure.MongoDB;
using TicketTack.Shared.Models;

namespace TicketTack.EventService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IMongoRepository<Event> _eventRepository;
        private readonly ILogger<EventController> _logger;

        public EventController(IMongoRepository<Event> eventRepository, ILogger<EventController> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetAll()
        {
            try
            {
                var events = await _eventRepository.GetAllAsync();
                var eventDtos = events.Select(MapToEventDto).ToList();

                return Ok(eventDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving events");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventDto>> GetById(string id)
        {
            try
            {
                var @event = await _eventRepository.GetByIdAsync(id);

                if (@event == null)
                    return NotFound();

                var eventDto = MapToEventDto(@event);
                return Ok(eventDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving event with id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,EventCreator")]
        public async Task<ActionResult<EventDto>> Create(CreateEventDto createEventDto)
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID is missing from token.");
                }

                var @event = new Event
                {
                    Name = createEventDto.Name,
                    Description = createEventDto.Description,
                    StartDate = createEventDto.StartDate,
                    EndDate = createEventDto.EndDate,
                    Location = createEventDto.Location,
                    TotalTickets = createEventDto.TotalTickets,
                    TicketsAvailable = createEventDto.TotalTickets,
                    Price = createEventDto.Price,
                    OrganizerId = userId,
                    IsPublished = createEventDto.IsPublished
                };

                await _eventRepository.AddAsync(@event);

                var eventDto = MapToEventDto(@event);
                return CreatedAtAction(nameof(GetById), new { id = @event.Id }, eventDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,EventCreator")]
        public async Task<ActionResult<EventDto>> Update(string id, UpdateEventDto updateEventDto)
        {
            try
            {
                var @event = await _eventRepository.GetByIdAsync(id);
                if (@event == null)
                    return NotFound();

                var userId = User.FindFirst("UserId")?.Value;
                var userRole = User.FindFirst("Role")?.Value;

                if (@event.OrganizerId != userId && userRole != "Admin")
                    return Forbid();

                @event.Name = updateEventDto.Name;
                @event.Description = updateEventDto.Description;
                @event.StartDate = updateEventDto.StartDate;
                @event.EndDate = updateEventDto.EndDate;
                @event.Location = updateEventDto.Location;

                if (@event.TotalTickets != updateEventDto.TotalTickets)
                {
                    var soldTickets = @event.TotalTickets - @event.TicketsAvailable;
                    @event.TotalTickets = updateEventDto.TotalTickets;
                    @event.TicketsAvailable = Math.Max(0, updateEventDto.TotalTickets - soldTickets);
                }

                @event.Price = updateEventDto.Price;
                @event.IsPublished = updateEventDto.IsPublished;

                await _eventRepository.UpdateAsync(@event);

                var eventDto = MapToEventDto(@event);
                return Ok(eventDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event with id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,EventCreator")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var @event = await _eventRepository.GetByIdAsync(id);
                if (@event == null)
                    return NotFound();

                var userId = User.FindFirst("UserId")?.Value;
                var userRole = User.FindFirst("Role")?.Value;

                if (@event.OrganizerId != userId && userRole != "Admin")
                    return Forbid();

                await _eventRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event with id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        private EventDto MapToEventDto(Event @event)
        {
            return new EventDto
            {
                Id = @event.Id,
                Name = @event.Name,
                Description = @event.Description,
                StartDate = @event.StartDate,
                EndDate = @event.EndDate ?? DateTime.MinValue,
                Location = @event.Location,
                TotalTickets = @event.TotalTickets,
                TicketsAvailable = @event.TicketsAvailable,
                Price = @event.Price,
                OrganizerId = @event.OrganizerId,
                Categories = new List<string>(), // À implémenter si nécessaire
                IsPublished = @event.IsPublished,
                Status = GetEventStatus(@event)
            };
        }

        private string GetEventStatus(Event @event)
        {
            var now = DateTime.UtcNow;

            if (!@event.IsPublished)
                return "Draft";

            if (@event.StartDate > now)
                return "Upcoming";

            if (@event.EndDate.HasValue && @event.EndDate.Value < now)
                return "Ended";

            if (@event.TicketsAvailable <= 0)
                return "SoldOut";

            return "Active";
        }
    }
}