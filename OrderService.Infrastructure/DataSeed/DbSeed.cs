using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Infrastructure.DataSeed
{
    public static class DbSeed
    {
        public static readonly Guid ProductSeedId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        public static readonly Guid ProductTwoSeedId = Guid.Parse("835c7288-b80d-4e61-9f39-cd8bc58dd668");
        public static readonly Guid ProductThreeSeedId = Guid.Parse("8f6a6044-7ac0-4d17-8148-f0f520d571e5");
        public static readonly Guid OrderSeedId = Guid.Parse("76ba2048-edfd-4ed5-a8a4-fe8ae40e492f");
        public static async Task<OrderDbContext> GetInMemoryOrderDbContext()
        {
            var orderDbContext = new OrderDbContext(
                new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: "OrderDb")
                .EnableSensitiveDataLogging().Options);

            orderDbContext.Database.EnsureDeleted();
            orderDbContext.Database.EnsureCreated();

            await AddSeedProducts(orderDbContext);
            await AddSeedOrders(orderDbContext);

            return orderDbContext;
        }

        public static async Task AddSeedProducts(OrderDbContext? orderDbContext)
        {
            if (orderDbContext != null && !orderDbContext.Products.Any())
            {
                orderDbContext.Products.Add(new Product { Id = ProductSeedId, Name = "Brownie", Price = 6, Stock = 1000 });
                orderDbContext.Products.Add(new Product { Id = ProductTwoSeedId, Name = "Water", Price = 2, Stock = 2000 });
                orderDbContext.Products.Add(new Product { Id = ProductThreeSeedId, Name = "Coffee", Price = 3, Stock = 3000 });

                await orderDbContext.SaveChangesAsync();
            }
        }

        public static async Task AddSeedOrders(OrderDbContext? orderDbContext)
        {
            if (orderDbContext != null && !orderDbContext.Orders.Any())
            {
                orderDbContext.Orders.Add(new Order()
                {
                    Id = OrderSeedId,
                    CustomerName = "Name",
                    AddressLine = "AddressLine",
                    AddressName = "AddressName",
                    EirCode = "EirCode",
                    County = "County",
                    OrderItems = new List<OrderItem>
                        {
                            new OrderItem
                            {
                                Product = orderDbContext.Products.Single(s => s.Id == ProductSeedId),
                                Quantity = 10,
                            },
                            new OrderItem
                            {
                                Product = orderDbContext.Products.Single(s => s.Id == ProductTwoSeedId),
                                Quantity = 10,
                            },
                            new OrderItem
                            {
                                Product = orderDbContext.Products.Single(s => s.Id == ProductThreeSeedId),
                                Quantity = 10,
                            },
                        }
                });
                await orderDbContext.SaveChangesAsync();

                for (var i = 0; i < 10; i++)
                {
                    orderDbContext.Orders.Add(new Order()
                    {
                        CustomerName = "Name",
                        AddressLine = "AddressLine",
                        AddressName = "AddressName",
                        EirCode = "EirCode",
                        County = "County",
                        OrderItems = new List<OrderItem>
                        {
                            new OrderItem
                            {
                                Product = new Product
                                {
                                    Name= "Name",
                                    Price = 10,
                                },
                                Quantity = 10,
                            }
                        }
                    });
                }
                await orderDbContext.SaveChangesAsync();
            }
        }
    }
}
