using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventService.Controllers;
using EventService.Models;
using EventService.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Linq;

namespace EventService.Tests
{
    public class EventControllerTests
    {
        private readonly Mock<IEventService> _mockEventService;
        private readonly EventController _controller;

        public EventControllerTests()
        {
            _mockEventService = new Mock<IEventService>();
            _controller = new EventController(_mockEventService.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllEvents()
        {
            // Arrange
            var expectedEvents = new List<Event>
            {
                new Event { Id = "1", Name = "Concert A", Date = DateTime.Now.AddDays(10), MaxCapacity = 100 },
                new Event { Id = "2", Name = "Concert B", Date = DateTime.Now.AddDays(20), MaxCapacity = 200 }
            };

            _mockEventService.Setup(service => service.GetAllEventsAsync())
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedEvents = Assert.IsAssignableFrom<IEnumerable<Event>>(okResult.Value);
            Assert.Equal(2, returnedEvents.Count());
        }

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnEvent()
        {
            // Arrange
            var eventId = "1";
            var expectedEvent = new Event { Id = eventId, Name = "Concert A", Date = DateTime.Now.AddDays(10), MaxCapacity = 100 };

            _mockEventService.Setup(service => service.GetEventByIdAsync(eventId))
                .ReturnsAsync(expectedEvent);

            // Act
            var result = await _controller.GetById(eventId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedEvent = Assert.IsType<Event>(okResult.Value);
            Assert.Equal(eventId, returnedEvent.Id);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var eventId = "999";
            _mockEventService.Setup(service => service.GetEventByIdAsync(eventId))
                .ReturnsAsync((Event)null);

            // Act
            var result = await _controller.GetById(eventId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_WithValidEvent_ShouldReturnCreatedEvent()
        {
            // Arrange
            var newEvent = new Event { Name = "New Concert", Date = DateTime.Now.AddDays(30), MaxCapacity = 300 };
            var createdEvent = new Event { Id = "3", Name = "New Concert", Date = DateTime.Now.AddDays(30), MaxCapacity = 300 };

            _mockEventService.Setup(service => service.CreateEventAsync(It.IsAny<Event>()))
                .ReturnsAsync(createdEvent);

            // Act
            var result = await _controller.Create(newEvent);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedEvent = Assert.IsType<Event>(createdResult.Value);
            Assert.Equal("3", returnedEvent.Id);
            Assert.Equal(newEvent.Name, returnedEvent.Name);
        }

        [Fact]
        public async Task Update_WithValidEvent_ShouldReturnUpdatedEvent()
        {
            // Arrange
            var eventId = "1";
            var eventToUpdate = new Event { Id = eventId, Name = "Updated Concert", Date = DateTime.Now.AddDays(15), MaxCapacity = 150 };

            _mockEventService.Setup(service => service.UpdateEventAsync(eventId, It.IsAny<Event>()))
                .ReturnsAsync(eventToUpdate);

            // Act
            var result = await _controller.Update(eventId, eventToUpdate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedEvent = Assert.IsType<Event>(okResult.Value);
            Assert.Equal(eventId, returnedEvent.Id);
            Assert.Equal(eventToUpdate.Name, returnedEvent.Name);
        }

        [Fact]
        public async Task Update_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var eventId = "999";
            var eventToUpdate = new Event { Id = eventId, Name = "Updated Concert", Date = DateTime.Now.AddDays(15), MaxCapacity = 150 };

            _mockEventService.Setup(service => service.UpdateEventAsync(eventId, It.IsAny<Event>()))
                .ReturnsAsync((Event)null);

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