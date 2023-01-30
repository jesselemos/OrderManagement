using AutoMapper;
using MediatR;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Commands.CancelOrder
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public CancelOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);
            if (order == null)
            {
                throw new ArgumentNullException($"OrderId: {request.OrderId} not found in our database.");
            }

            _mapper.Map(request, order, typeof(CancelOrderCommand), typeof(Order));

            order.OrderStatus = OrderStatus.Canceled;

            await _orderRepository.UpdateOrderAsync(order);

            return Unit.Value;
        }
    }
}
