using MediatR;

namespace OrderService.Application.Commands.CreateOrder
{
    public record CreateOrderNotification(CreateOrderCommand Order) : INotification;
}
