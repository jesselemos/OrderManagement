using MediatR;
using OrderService.API.Entities;

namespace OrderService.API.Queries.GetOrders
{
    public record GetOrderByIdQuery(int Id) : IRequest<Order>;
}
