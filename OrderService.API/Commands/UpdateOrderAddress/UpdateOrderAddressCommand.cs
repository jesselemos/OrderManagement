using MediatR;

namespace OrderService.API.Commands.UpdateOrderAddress
{
    public class UpdateOrderAddressCommand : IRequest
    {
        public Guid OrderId { get; set; }
        public string? AddressName { get; set; }
        public string? AddressLine { get; set; }
        public string? County { get; set; }
        public string? EirCode { get; set; }
    }
}
