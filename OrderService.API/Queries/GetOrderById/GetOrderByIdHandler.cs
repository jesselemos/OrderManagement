using MediatR;
using OrderService.API.DataStore;
using OrderService.API.Entities;

namespace OrderService.API.Queries.GetOrders
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, Order>
    {
        private readonly FakeDataStore _fakeDataStore;

        public GetOrderByIdHandler(FakeDataStore fakeDataStore) => _fakeDataStore = fakeDataStore;

        public async Task<Order> Handle(GetOrderByIdQuery request,
            CancellationToken cancellationToken) => await _fakeDataStore.GetOrderById(request.Id);
    }
}
