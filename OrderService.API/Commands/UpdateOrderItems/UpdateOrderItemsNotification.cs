using MediatR;

namespace OrderService.API.Commands.UpdateOrderItems
{
    public record UpdateOrderItemsNotification(UpdateOrderItemsCommand Order) : INotification;
}
