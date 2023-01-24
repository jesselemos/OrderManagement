namespace OrderService.API.Entities
{
    public class Order : EntityBase
    {
        //public Order() { 

        //}

        public string UserName { get; set; }
        public string Name { get; set; }
        public decimal TotalPrice { get; set; }

        // Delivery Address
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string AddressLine { get; set; }
        public string County { get; set; }
        public string EirCode { get; set; }
    }
}
