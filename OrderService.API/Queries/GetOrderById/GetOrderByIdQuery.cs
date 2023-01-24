using MediatR;
using OrderService.API.Entities;

namespace OrderService.API.Queries.GetOrders
{
    public record GetOrderByIdQuery(Guid Id) : IRequest<Order>;
}
