using MediatR;

namespace OrderService.Application.Commands.CancelOrder
{
    public record CancelOrderNotification(CancelOrderCommand Order) : INotification;
}
