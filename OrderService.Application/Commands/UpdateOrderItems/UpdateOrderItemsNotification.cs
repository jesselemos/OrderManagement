using MediatR;

namespace OrderService.Application.Commands.UpdateOrderItems
{
    public record UpdateOrderItemsNotification(UpdateOrderItemsCommand Order) : INotification;
}
