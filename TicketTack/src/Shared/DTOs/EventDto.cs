using System.ComponentModel.DataAnnotations;

namespace TicketTack.Shared.DTOs
{
    public class EventDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public int TotalTickets { get; set; }
        public int TicketsAvailable { get; set; }
        public decimal Price { get; set; }
        public string OrganizerId { get; set; }
        public List<string> Categories { get; set; } = new List<string>();
        public bool IsPublished { get; set; }
        public string Status { get; set; }
    }

    public class CreateEventDto
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int TotalTickets { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public List<string> Categories { get; set; } = new List<string>();

        public bool IsPublished { get; set; }
    }

    public class UpdateEventDto
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int TotalTickets { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public List<string> Categories { get; set; } = new List<string>();

        public bool IsPublished { get; set; }
    }
}
