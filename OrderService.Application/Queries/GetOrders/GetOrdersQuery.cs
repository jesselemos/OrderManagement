using MediatR;
using OrderService.Domain.Entities;

namespace OrderService.Application.Queries.GetOrders
{
    public record GetOrdersQuery(int Take, int Skip) : IRequest<IEnumerable<Order>>;
}
