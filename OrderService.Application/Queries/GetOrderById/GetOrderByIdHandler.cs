using MediatR;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Queries.GetOrderById
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, Order?>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdHandler(IOrderRepository orderRepository) => _orderRepository = orderRepository;

        public async Task<Order?> Handle(GetOrderByIdQuery request,
            CancellationToken cancellationToken) => await _orderRepository.GetOrderByIdAsync(request.OrderId);
    }
}
