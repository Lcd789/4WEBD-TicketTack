using Shared.Models.Enums;
using System;

namespace TicketTack.Shared.Models
{
    public class Ticket : BaseEntity
    {
        public string EventId { get; set; }
        public string EventName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string TicketNumber { get; set; } // Pour la sécurité et la vérification
        public decimal PurchasePrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public TicketStatus Status { get; set; }
        public bool IsUsed { get; set; } = false;
        public string QRCodeData { get; set; } // Pour générer un QR code
        public string PaymentMethod { get; set; }
        public string PaymentReference { get; set; }
    }
}
