namespace OrderService.API.Entities
{
    public class OrderItem : EntityBase
    {
        public OrderItem()
        {
            Id = Guid.NewGuid();
        }

        public Order? Order { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
    }
}
