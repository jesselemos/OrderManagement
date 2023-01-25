namespace OrderService.API.Entities
{
    public class Product : EntityBase
    {
        public Product()
        {
            Id = new();
        }

        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
