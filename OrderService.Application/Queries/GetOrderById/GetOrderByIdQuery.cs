using MediatR;
using OrderService.Domain.Entities;

namespace OrderService.Application.Queries.GetOrderById
{
    public record GetOrderByIdQuery(Guid OrderId) : IRequest<Order?>;
}
