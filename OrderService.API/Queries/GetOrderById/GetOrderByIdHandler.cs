using MediatR;
using OrderService.API.Entities;
using OrderService.API.Repositories;

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
