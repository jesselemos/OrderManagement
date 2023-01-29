namespace OrderService.Domain.Entities
{
    public class OrderItem : EntityBase
    {
        public OrderItem()
        {
            Id = Guid.Empty;
        }

        public Product Product { get; set; } = new();
        public int Quantity { get; set; }
    }
}
