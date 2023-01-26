using MediatR;
using OrderService.API.Entities;

namespace OrderService.API.Queries.GetOrders
{
    public record GetOrdersQuery(int Take, int Skip) : IRequest<IEnumerable<Order>>;
}
