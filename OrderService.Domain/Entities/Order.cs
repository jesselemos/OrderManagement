namespace OrderService.Domain.Entities
{
    public class Order : EntityBase
    {
        public Order()
        {
            Id = Guid.NewGuid();
        }

        public string OrderStatus { get; set; } = Entities.OrderStatus.Created;
        public string? CustomerName { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
        public string? AddressName { get; set; }
        public string? AddressLine { get; set; }
        public string? County { get; set; }
        public string? EirCode { get; set; }
        public decimal Total => OrderItems.Any()
            ? OrderItems
                .Select(x => x.Product.Price * x.Quantity)
                .Sum()
            : default;
    }
}
