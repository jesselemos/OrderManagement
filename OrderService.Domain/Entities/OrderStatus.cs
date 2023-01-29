namespace OrderService.Domain.Entities
{
    public static class OrderStatus
    {
        public const string Created = "Created";
        public const string InTransit = "InTransit";
        public const string Delivered = "Delivered";
        public const string Returned = "Returned";
        public const string Canceled = "Canceled";
    }
}
