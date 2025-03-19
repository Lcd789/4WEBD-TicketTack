namespace TicketTack.Shared.DTOs
{
    public class EventDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string VenueId { get; set; }
        public string VenueName { get; set; }
        public int TotalTickets { get; set; }
        public int AvailableTickets { get; set; }
        public decimal TicketPrice { get; set; }
        public List<string> Categories { get; set; }
        public string ImageUrl { get; set; }
        public string Status { get; set; }
    }

    public class CreateEventDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string VenueId { get; set; }
        public int TotalTickets { get; set; }
        public decimal TicketPrice { get; set; }
        public List<string> Categories { get; set; }
        public string ImageUrl { get; set; }
    }

    public class UpdateEventDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string VenueId { get; set; }
        public decimal TicketPrice { get; set; }
        public List<string> Categories { get; set; }
        public string ImageUrl { get; set; }
        public string Status { get; set; }
    }
}
