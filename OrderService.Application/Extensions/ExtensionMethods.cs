using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OrderService.Application.Behaviours;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.DataSeed;
using OrderService.Infrastructure.Repositories;
using System.Reflection;

namespace OrderService.Application.Extensions
{
    public static class ExtensionMethods
    {
        public static T? DeepCopy<T>(this T self)
        {
            var serialized = JsonConvert.SerializeObject(self);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddDbContext<OrderDbContext>(opt =>
            opt.UseInMemoryDatabase("OrderDb").EnableSensitiveDataLogging());
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddHealthChecks().AddDbContextCheck<OrderDbContext>();
            return services;
        }
    }
}
