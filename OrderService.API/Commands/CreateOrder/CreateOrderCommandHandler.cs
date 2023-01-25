using AutoMapper;
using MediatR;
using OrderService.API.Entities;
using OrderService.API.Repositories;

namespace OrderService.API.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Order>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var newOrder = _mapper.Map<Order>(request);


            await _orderRepository.CreateOrder(newOrder);
            return newOrder;
        }
    }
}
