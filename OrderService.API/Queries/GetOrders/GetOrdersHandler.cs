using MediatR;
using OrderService.API.DataStore;
using OrderService.API.Entities;

namespace OrderService.API.Queries.GetOrders
{
    public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, IEnumerable<Order>>
    {
        private readonly FakeDataStore _fakeDataStore;

        public GetOrdersHandler(FakeDataStore fakeDataStore) => _fakeDataStore = fakeDataStore;

        public async Task<IEnumerable<Order>> Handle(GetOrdersQuery request,
            CancellationToken cancellationToken) => await _fakeDataStore.GetAllOrders();
    }
}
