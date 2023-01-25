using AutoMapper;
using MediatR;
using OrderService.API.Entities;
using OrderService.API.Exceptions;
using OrderService.API.Repositories;

namespace OrderService.API.Commands.CreateOrder
{
    public class CancelOrderCommandCommandHandler : IRequestHandler<CancelOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public CancelOrderCommandCommandHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);
            if (order == null)
            {
                throw new NotFoundException(nameof(Order), request.OrderId);
            }

            _mapper.Map(request, order, typeof(CancelOrderCommand), typeof(Order));

            order.OrderStatus = OrderStatus.Canceled;

            await _orderRepository.UpdateOrderAsync(order);

            return Unit.Value;
        }
    }
}
