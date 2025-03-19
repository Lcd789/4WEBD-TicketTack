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
        public string Status { get; set; }
    }

    public class PurchaseTicketDto
    {
        public string EventId { get; set; }
        public int Quantity { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentDetails { get; set; }
    }
}