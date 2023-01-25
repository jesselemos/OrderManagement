namespace OrderService.API.Entities
{
    public class OrderItem : EntityBase
    {
        public OrderItem()
        {
            Id = new();
        }

        public Product Product { get; set; } = new();
        public int Quantity { get; set; }
    }
}
