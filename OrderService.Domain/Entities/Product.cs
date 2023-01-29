namespace OrderService.Domain.Entities
{
    public class Product : EntityBase
    {
        public Product()
        {
            Id = Guid.NewGuid();
        }

        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
