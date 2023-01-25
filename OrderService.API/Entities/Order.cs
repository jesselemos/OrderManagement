namespace OrderService.API.Entities
{
    public class Order : EntityBase
    {
        public Order()
        {
            Id = Guid.NewGuid();
        }

        public string? CustomerName { get; set; }
        public decimal TotalPrice { get; set; }
        //TODO::Map new entities to datatable
        // public Product? Product { get; set; }
        public Guid? ProductId { get; set; }
        public string? AddressName { get; set; }
        public string? AddressLine { get; set; }
        public string? County { get; set; }
        public string? EirCode { get; set; }
    }
}
