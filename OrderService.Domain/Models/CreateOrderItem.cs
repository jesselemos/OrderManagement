namespace OrderService.Domain.Models
{
    public class CreateOrderItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
