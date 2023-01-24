namespace OrderService.API.Commands.CreateOrder
{
    public class CreateOrderCommand : ICommand
    {
        public CreateOrderCommand()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        // Delivery Address
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string AddressLine { get; set; }
        public string County { get; set; }
        public string EirCode { get; set; }
    }
}
