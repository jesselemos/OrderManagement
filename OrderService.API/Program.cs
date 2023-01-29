using Microsoft.OpenApi.Models;
using OrderService.Application.Extensions;
using OrderService.Infrastructure.DataSeed;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplication();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Order Management API",
        Description = "API for managing orders"
    });
});

var app = builder.Build();
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetService<IDbInitializer>();
    dbInitializer?.SeedData();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
#pragma warning disable S1118 // Utility classes should not have public constructors
public partial class Program { } //This line is necessary for Integration Tests
#pragma warning restore S1118 // Utility classes should not have public constructors
