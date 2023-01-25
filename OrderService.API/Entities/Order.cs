namespace OrderService.API.Entities
{
    public class Order : EntityBase
    {
        public Order()
        {
            Id = Guid.NewGuid();
        }
        public string OrderStatus { get; set; } = Entities.OrderStatus.Created;
        public string? CustomerName { get; set; }
        // public decimal TotalPrice { get; set; }

        // public Product? Product { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        // public Guid? ProductId { get; set; }
        public string? AddressName { get; set; }
        public string? AddressLine { get; set; }
        public string? County { get; set; }
        public string? EirCode { get; set; }
    }
}
