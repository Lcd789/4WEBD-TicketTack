using System;
using System.Collections.Generic;

namespace TicketTack.Shared.Models
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; } // Admin, EventCreator, User
        public List<string> PurchasedTickets { get; set; } = new List<string>();

        public string FullName => $"{FirstName} {LastName}";
    }
}
