using MediatR;
using OrderService.API.Entities;

namespace OrderService.API.Commands.CreateOrder
{
    public record CreateOrderCommand(Order Order) : IRequest;
}
