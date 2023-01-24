using MediatR;
using OrderService.API.Entities;

namespace OrderService.API.Commands.CreateOrder
{
    public record CreateOrderNotification(Order Order) : INotification;
}
