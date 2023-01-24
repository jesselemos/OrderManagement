namespace OrderService.API.Entities
{
    public class Address : EntityBase
    {
        public Address()
        {
            Id = Guid.NewGuid();
        }

        public string? Name { get; set; }
        public string? AddressLine { get; set; }
        public string? County { get; set; }
        public string? EirCode { get; set; }
    }
}
