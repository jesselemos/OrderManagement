namespace OrderService.API.Entities
{
    public class Product : EntityBase
    {
        public Product()
        {
            Id = Guid.NewGuid();
        }

        public string? ProductName { get; set; }
    }
}
