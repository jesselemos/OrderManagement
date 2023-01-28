using MediatR;
using OrderService.API.Entities;

namespace OrderService.API.Queries.GetOrderById
{
    public record GetOrderByIdQuery(Guid OrderId) : IRequest<Order?>;
}
