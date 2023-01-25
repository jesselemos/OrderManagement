using MediatR;
using OrderService.API.Entities;

namespace OrderService.API.Queries.GetOrders
{
    public record GetOrdersQuery(int take, int skip) : IRequest<IEnumerable<Order>>;
}
