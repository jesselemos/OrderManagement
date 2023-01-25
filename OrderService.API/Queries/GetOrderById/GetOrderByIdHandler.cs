using MediatR;
using OrderService.API.Entities;
using OrderService.API.Repositories;

namespace OrderService.API.Queries.GetOrders
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, Order>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdHandler(IOrderRepository orderRepository) => _orderRepository = orderRepository;

        public async Task<Order> Handle(GetOrderByIdQuery request,
            CancellationToken cancellationToken) => await _orderRepository.GetOrderByIdAsync(request.Id);
    }
}
