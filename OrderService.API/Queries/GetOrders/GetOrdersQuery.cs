using MediatR;
using OrderService.API.Entities;

namespace OrderService.API.Queries.GetOrders
{
    public record GetOrdersQuery : IRequest<IEnumerable<Order>>;
}
