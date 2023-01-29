using OrderService.Application.Commands.UpdateOrderItems;
using OrderService.Domain.Models;
using OrderService.API.Tests.UnitTests.Helpers;

namespace OrderService.API.Tests.UnitTests.Commands.UpdateOrderItems
{
    [TestFixture]
    public class UpdateOrderItemsCommandValidatorTests
    {
        [Test]
        public async Task UpdateOrderItemsCommandValidatorIsValid()
        {
            var validator = new UpdateOrderItemsCommandValidator();
            var command = new UpdateOrderItemsCommand()
            {
                OrderId = Guid.NewGuid(),
                OrderItems = new List<CreateOrderItem>
                    {
                        new CreateOrderItem
                        {
                            ProductId = DatabaseHelper.ProductSeedId,
                            Quantity = 10
                        }
                    }
            };

            var result = await validator.ValidateAsync(command);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsValid);
                Assert.That(!result.Errors.Any());
            });
        }

        [TestCase("OrderId")]
        [TestCase("OrderItems")]
        public async Task UpdateOrderItemsCommandValidatorContainsErrorMessageForRequiredFields(string field)
        {
            var result = await new UpdateOrderItemsCommandValidator().ValidateAsync(new UpdateOrderItemsCommand());

            Assert.Multiple(() =>
            {
                Assert.That(!result.IsValid);
                Assert.That(result.Errors.Any(a => a.ErrorMessage == $"{{{field}}} is required."));
            });
        }
    }
}
