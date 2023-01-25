using AutoMapper;
using MediatR;
using OrderService.API.Entities;
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
            var orderToUpdate = await _orderRepository.GetOrderByIdAsync(request.OrderId);

            _mapper.Map(request, orderToUpdate, typeof(CancelOrderCommand), typeof(Order));

            orderToUpdate.OrderStatus = OrderStatus.Canceled;

            await _orderRepository.UpdateOrderAsync(orderToUpdate);

            return Unit.Value;
        }
    }
}
