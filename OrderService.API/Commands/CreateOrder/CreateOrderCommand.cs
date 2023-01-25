using MediatR;
using OrderService.API.Models;

namespace OrderService.API.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<Guid>
    {
        public string? CustomerName { get; set; }
        public List<CreateOrderItem> OrderItems { get; set; } = new();
        public string? AddressName { get; set; }
        public string? AddressLine { get; set; }
        public string? County { get; set; }
        public string? EirCode { get; set; }
    }
}
