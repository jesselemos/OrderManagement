using MediatR;
using OrderService.API.DataStore;

namespace OrderService.API.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Unit>
    {
        private readonly FakeDataStore _fakeDataStore;
        public CreateOrderCommandHandler(FakeDataStore fakeDataStore) => _fakeDataStore = fakeDataStore;

        public async Task<Unit> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            await _fakeDataStore.AddOrder(request.Order);
            return Unit.Value;
        }
    }
}
