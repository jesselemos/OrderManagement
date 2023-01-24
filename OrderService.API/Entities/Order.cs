namespace OrderService.API.Entities
{
    public class Order : EntityBase
    {
        public Order()
        {
            Id = Guid.NewGuid();
        }

        public Guid ProductId { get; set; }

        public string? CustomerName { get; set; }
        public decimal TotalPrice { get; set; }

        // Delivery Address
        public string? AddressName { get; set; }
        public string? EmailAddress { get; set; }
        public string? AddressLine { get; set; }
        public string? County { get; set; }
        public string? EirCode { get; set; }
    }
}
