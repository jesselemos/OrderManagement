using AutoMapper;
using OrderService.Application.Mappers;

namespace OrderService.Tests.UnitTests.Helpers
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
