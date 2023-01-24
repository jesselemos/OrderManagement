using MediatR;
using OrderService.API.Repositories;

namespace OrderService.API.Commands.CreateOrder
{
    public class CreateOrderNotificationHandler : INotificationHandler<CreateOrderNotification>
    {
        private readonly FakeDataStore _fakeDataStore;

        public CreateOrderNotificationHandler(FakeDataStore fakeDataStore) => _fakeDataStore = fakeDataStore;

        public async Task Handle(CreateOrderNotification notification, CancellationToken cancellationToken)
        {
            await _fakeDataStore.EventOccured(notification.Order, "Order Created");
            await Task.CompletedTask;
        }
    }
}
