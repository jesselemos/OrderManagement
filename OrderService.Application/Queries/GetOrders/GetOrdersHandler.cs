using MediatR;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Application.Queries.GetOrders
{
    public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, IEnumerable<Order>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersHandler(IOrderRepository orderRepository) => _orderRepository = orderRepository;

        public async Task<IEnumerable<Order>> Handle(GetOrdersQuery request,
            CancellationToken cancellationToken) => await _orderRepository.GetAllOrdersAsync(request.Take, request.Skip);
    }
}
