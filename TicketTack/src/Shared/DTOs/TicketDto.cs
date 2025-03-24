using System.ComponentModel.DataAnnotations;

namespace TicketTack.Shared.DTOs
{
    public class TicketDto
    {
        public string Id { get; set; }
        public string EventId { get; set; }
        public string EventName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public decimal Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string TicketNumber { get; set; }
        public string QRCodeData { get; set; }
        public string Status { get; set; }
        public bool IsUsed { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentReference { get; set; }
    }

    public class PurchaseTicketDto
    {
        [Required]
        public string EventId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        [Required]
        public string PaymentDetails { get; set; }
    }
}