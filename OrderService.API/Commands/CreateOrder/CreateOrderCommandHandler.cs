using MediatR;
using OrderService.API.Entities;
using OrderService.API.Repositories;

namespace OrderService.API.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Order>
    {
        private readonly IOrderRepository _orderRepository;
        public CreateOrderCommandHandler(IOrderRepository orderRepository) => _orderRepository = orderRepository;

        public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            await _orderRepository.CreateOrder(request.Order);
            return request.Order;
        }
    }
}
