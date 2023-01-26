using AutoMapper;
using OrderService.API.Mappers;

namespace OrderService.API.Tests.UnitTests.Helpers
{
    public static class AutoMapperHelper
    {
        public static IMapper CreateMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapping>();
            }).CreateMapper();
        }
    }
}
