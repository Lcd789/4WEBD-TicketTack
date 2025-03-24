using System;
using System.Collections.Generic;

namespace TicketTack.Shared.Models
{
    public class Event : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Location { get; set; }
        public int TotalTickets { get; set; }
        public int TicketsAvailable { get; set; }
        public decimal Price { get; set; }
        public string OrganizerId { get; set; }
        public List<string> Categories { get; set; } = new List<string>();
        public bool IsPublished { get; set; } = false;

        public void ReduceAvailableTickets(int quantity)
        {
            if (TicketsAvailable >= quantity)
                TicketsAvailable -= quantity;
            else
                throw new InvalidOperationException("Not enough tickets available.");
        }
    }
}
