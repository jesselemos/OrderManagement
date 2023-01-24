using MediatR;
using OrderService.API.Repositories;

namespace OrderService.API.Commands.CreateOrder
{
    public class CreateOrderNotificationHandler : INotificationHandler<CreateOrderNotification>
    {
        private readonly IOrderRepository _orderRepository;

        public CreateOrderNotificationHandler(IOrderRepository orderRepository) => _orderRepository = orderRepository;

        public async Task Handle(CreateOrderNotification notification, CancellationToken cancellationToken)
        {
            await _orderRepository.EventOccured(notification.Order, "Order Created");
            await Task.CompletedTask;
        }
    }
}
