using MediatR;

namespace OrderService.API.Commands.CreateOrder
{
    public record CreateOrderNotification(CreateOrderCommand Order) : INotification;
}
