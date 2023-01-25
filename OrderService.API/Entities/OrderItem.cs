namespace OrderService.API.Entities
{
    public class OrderItem : EntityBase
    {
        public OrderItem()
        {
            Id = Guid.NewGuid();
        }

        public Product? Product { get; set; }
        public int Quantity { get; set; }
    }
}
