using MediatR;
using OrderService.Domain.Entities;
using OrderService.Domain.Models;

namespace OrderService.Application.Commands.UpdateOrderItems
{
    public class UpdateOrderItemsCommand : IRequest<Order>
    {
        public Guid OrderId { get; set; }
        public List<CreateOrderItem> OrderItems { get; set; } = new();

        [System.Text.Json.Serialization.JsonIgnore]
        public Order? PreviousOrder { get; set; }
    }
}
