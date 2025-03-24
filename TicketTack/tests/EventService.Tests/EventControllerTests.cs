using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketTack.EventService.Controllers;
using TicketTack.Shared.Models;
using TicketTack.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Linq;

namespace EventService.Tests
{
    public class EventControllerTests
    {
        private readonly Mock<EventService> _mockEventService;
        private readonly EventController _controller;

        public EventControllerTests()
        {
            _mockEventService = new Mock<EventService>();
            _controller = new EventController(_mockEventService.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllEvents()
        {
            // Arrange
            var expectedEvents = new List<EventDto>
            {
                new EventDto {
                    Id = "1",
                    Name = "Concert A",
                    StartDate = DateTime.Now.AddDays(10),
                    EndDate = DateTime.Now.AddDays(11),
                    Location = "Venue A",
                    TotalTickets = 100,
                    TicketsAvailable = 100,
                    Price = 25.00m,
                    Categories = new List<string> { "Music" },
                    IsPublished = true
                },
                new EventDto {
                    Id = "2",
                    Name = "Concert B",
                    StartDate = DateTime.Now.AddDays(15),
                    EndDate = DateTime.Now.AddDays(16),
                    Location = "Venue B",
                    TotalTickets = 200,
                    TicketsAvailable = 200,
                    Price = 35.00m,
                    Categories = new List<string> { "Music" },
                    IsPublished = true
                }
            };

            _mockEventService.Setup(service => service.GetAllEventsAsync())
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedEvents = Assert.IsAssignableFrom<IEnumerable<EventDto>>(okResult.Value);
            Assert.Equal(2, returnedEvents.Count());
        }

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnEvent()
        {
            // Arrange
            var eventId = "1";
            var expectedEvent = new EventDto
            {
                Id = eventId,
                Name = "Concert A",
                StartDate = DateTime.Now.AddDays(10),
                EndDate = DateTime.Now.AddDays(11),
                Location = "Venue A",
                TotalTickets = 100,
                TicketsAvailable = 100,
                Price = 25.00m,
                Categories = new List<string> { "Music" },
                IsPublished = true
            };

            _mockEventService.Setup(service => service.GetEventByIdAsync(eventId))
                .ReturnsAsync(expectedEvent);

            // Act
            var result = await _controller.GetById(eventId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedEvent = Assert.IsType<EventDto>(okResult.Value);
            Assert.Equal(eventId, returnedEvent.Id);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var eventId = "999";
            _mockEventService.Setup(service => service.GetEventByIdAsync(eventId))
                .ReturnsAsync((EventDto)null);

            // Act
            var result = await _controller.GetById(eventId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_WithValidEvent_ShouldReturnCreatedEvent()
        {
            // Arrange
            var newEvent = new CreateEventDto
            {
                Name = "New Concert",
                StartDate = DateTime.Now.AddDays(30),
                EndDate = DateTime.Now.AddDays(31),
                Location = "New Venue",
                TotalTickets = 300,
                Price = 45.00m,
                Categories = new List<string> { "Music", "Festival" },
                IsPublished = true
            };

            var createdEvent = new EventDto
            {
                Id = "3",
                Name = "New Concert",
                StartDate = DateTime.Now.AddDays(30),
                EndDate = DateTime.Now.AddDays(31),
                Location = "New Venue",
                TotalTickets = 300,
                TicketsAvailable = 300,
                Price = 45.00m,
                Categories = new List<string> { "Music", "Festival" },
                IsPublished = true
            };

            _mockEventService.Setup(service => service.CreateEventAsync(It.IsAny<CreateEventDto>()))
                .ReturnsAsync(createdEvent);

            // Act
            var result = await _controller.Create(newEvent);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedEvent = Assert.IsType<EventDto>(createdResult.Value);
            Assert.Equal("3", returnedEvent.Id);
            Assert.Equal(newEvent.Name, returnedEvent.Name);
        }

        [Fact]
        public async Task Update_WithValidEvent_ShouldReturnUpdatedEvent()
        {
            // Arrange
            var eventId = "1";
            var eventToUpdate = new UpdateEventDto
            {
                Name = "Updated Concert",
                StartDate = DateTime.Now.AddDays(15),
                EndDate = DateTime.Now.AddDays(16),
                Location = "Updated Venue",
                TotalTickets = 150,
                Price = 30.00m,
                Categories = new List<string> { "Music", "Live" },
                IsPublished = true
            };

            var updatedEvent = new EventDto
            {
                Id = eventId,
                Name = "Updated Concert",
                StartDate = DateTime.Now.AddDays(15),
                EndDate = DateTime.Now.AddDays(16),
                Location = "Updated Venue",
                TotalTickets = 150,
                TicketsAvailable = 150,
                Price = 30.00m,
                Categories = new List<string> { "Music", "Live" },
                IsPublished = true
            };

            _mockEventService.Setup(service => service.UpdateEventAsync(eventId, It.IsAny<UpdateEventDto>()))
                .ReturnsAsync(updatedEvent);

            // Act
            var result = await _controller.Update(eventId, eventToUpdate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedEvent = Assert.IsType<EventDto>(okResult.Value);
            Assert.Equal(eventId, returnedEvent.Id);
            Assert.Equal(eventToUpdate.Name, returnedEvent.Name);
        }

        [Fact]
        public async Task Update_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var eventId = "999";
            var eventToUpdate = new UpdateEventDto
            {
                Name = "Updated Concert",
                StartDate = DateTime.Now.AddDays(15),
                EndDate = DateTime.Now.AddDays(16),
                Location = "Updated Venue",
                TotalTickets = 150,
                Price = 30.00m,
                Categories = new List<string> { "Music", "Live" },
                IsPublished = true
            };

            _mockEventService.Setup(service => service.UpdateEventAsync(eventId, It.IsAny<UpdateEventDto>()))
                .ReturnsAsync((EventDto)null);

            // Act
            var result = await _controller.Update(eventId, eventToUpdate);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Delete_WithValidId_ShouldReturnNoContent()
        {
            // Arrange
            var eventId = "1";
            _mockEventService.Setup(service => service.DeleteEventAsync(eventId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(eventId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var eventId = "999";
            _mockEventService.Setup(service => service.DeleteEventAsync(eventId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(eventId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}