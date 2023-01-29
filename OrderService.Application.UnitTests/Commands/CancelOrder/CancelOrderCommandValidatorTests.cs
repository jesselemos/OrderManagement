using OrderService.Application.Commands.CancelOrder;

namespace OrderService.Application.UnitTests.Commands.CancelOrder
{
    [TestFixture]
    public class CancelOrderCommandValidatorTests
    {
        [Test]
        public async Task CancelOrderCommandValidatorIsValid()
        {
            var validator = new CancelOrderCommandValidator();
            var command = new CancelOrderCommand()
            {
                OrderId = Guid.NewGuid()
            };

            var result = await validator.ValidateAsync(command);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsValid);
                Assert.That(!result.Errors.Any());
            });
        }

        [TestCase("OrderId")]
        public async Task CancelOrderCommandValidatorContainsErrorMessageForRequiredFields(string field)
        {
            var result = await new CancelOrderCommandValidator().ValidateAsync(new CancelOrderCommand());

            Assert.Multiple(() =>
            {
                Assert.That(!result.IsValid);
                Assert.That(result.Errors.Any(a => a.ErrorMessage == $"{{{field}}} is required."));
            });
        }
    }
}
