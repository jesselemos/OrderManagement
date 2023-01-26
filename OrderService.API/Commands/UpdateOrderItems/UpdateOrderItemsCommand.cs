using MediatR;
using OrderService.API.Models;

namespace OrderService.API.Commands.UpdateOrderItems
{
    public class UpdateOrderItemsCommand : IRequest
    {
        public Guid OrderId { get; set; }
        public List<CreateOrderItem> OrderItems { get; set; } = new();
    }
}
