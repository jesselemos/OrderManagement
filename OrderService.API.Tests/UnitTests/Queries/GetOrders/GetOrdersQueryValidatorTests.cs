using OrderService.API.Queries.GetOrders;

namespace OrderService.API.Tests.UnitTests.Queries.GetOrders
{
    [TestFixture]
    public class GetOrdersQueryValidatorTests
    {
        [TestCase(1, 0)]
        [TestCase(1, 1)]
        [TestCase(10, 10)]
        public async Task GetOrdersQueryValidatorIsValid(int take, int skip)
        {
            var validator = new GetOrdersQueryValidator();
            var result = await validator.ValidateAsync(new GetOrdersQuery(take, skip));

            Assert.Multiple(() =>
            {
                Assert.That(result.IsValid);
                Assert.That(!result.Errors.Any());
            });
        }

        [TestCase(0, -1)]
        [TestCase(-1, -1)]
        public async Task GetOrdersQueryValidatorContainsErrorMessagesWhenFieldsContainsInvalidValues(int take, int skip)
        {
            var result = await new GetOrdersQueryValidator().ValidateAsync(new GetOrdersQuery(take, skip));

            Assert.Multiple(() =>
            {
                Assert.That(!result.IsValid);
                Assert.That(result.Errors.Any(a => a.ErrorMessage == $"{{Take}} is required and should be greater than zero."));
                Assert.That(result.Errors.Any(a => a.ErrorMessage == $"{{Skip}} is required and should be greater than or equal to zero."));
            });
        }
    }
}
