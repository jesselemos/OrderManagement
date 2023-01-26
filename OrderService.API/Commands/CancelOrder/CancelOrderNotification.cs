using MediatR;

namespace OrderService.API.Commands.CancelOrder
{
    public record CancelOrderNotification(CancelOrderCommand Order) : INotification;
}
