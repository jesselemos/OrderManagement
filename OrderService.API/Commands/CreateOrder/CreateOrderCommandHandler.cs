using MediatR;
using OrderService.API.DataStore;
using OrderService.API.Entities;

namespace OrderService.API.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Order>
    {
        private readonly FakeDataStore _fakeDataStore;
        public CreateOrderCommandHandler(FakeDataStore fakeDataStore) => _fakeDataStore = fakeDataStore;

        public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            await _fakeDataStore.AddOrder(request.Order);
            return request.Order;
        }
    }
}
