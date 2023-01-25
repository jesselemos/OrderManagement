using AutoMapper;
using OrderService.API.Commands.CreateOrder;
using OrderService.API.Entities;

namespace OrderService.API.Mappers
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<CreateOrderCommand, Order>();
        }
    }
}
