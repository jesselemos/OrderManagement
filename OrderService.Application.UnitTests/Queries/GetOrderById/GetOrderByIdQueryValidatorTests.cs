using OrderService.Application.Queries.GetOrderById;

namespace OrderService.Application.UnitTests.Queries.GetOrderById
{
    [TestFixture]
    public class GetOrderByIdQueryValidatorTests
    {
        [Test]
        public async Task GetOrderByIdQueryValidatorIsValid()
        {
            var validator = new GetOrderByIdQueryValidator();
            var result = await validator.ValidateAsync(new GetOrderByIdQuery(Guid.NewGuid()));

            Assert.Multiple(() =>
            {
                Assert.That(result.IsValid);
                Assert.That(!result.Errors.Any());
            });
        }

        [TestCase("OrderId")]
        public async Task GetOrderByIdQueryValidatorContainsErrorMessageForRequiredFields(string field)
        {
            var result = await new GetOrderByIdQueryValidator().ValidateAsync(new GetOrderByIdQuery(Guid.Empty));

            Assert.Multiple(() =>
            {
                Assert.That(!result.IsValid);
                Assert.That(result.Errors.Any(a => a.ErrorMessage == $"{{{field}}} is required."));
            });
        }
    }
}
