using MediatR;
using OrderService.API.Entities;
using OrderService.API.Models;

namespace OrderService.API.Commands.UpdateOrderItems
{
    public class UpdateOrderItemsCommand : IRequest<Order>
    {
        public Guid OrderId { get; set; }
        public List<CreateOrderItem> OrderItems { get; set; } = new();

        [System.Text.Json.Serialization.JsonIgnore]
        public Order? PreviousOrder { get; set; }
    }
}
