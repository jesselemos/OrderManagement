using MediatR;
using OrderService.API.Entities;

namespace OrderService.API.Queries.GetOrderById
{
    public record GetOrderByIdQuery(Guid Id) : IRequest<Order?>;
}
