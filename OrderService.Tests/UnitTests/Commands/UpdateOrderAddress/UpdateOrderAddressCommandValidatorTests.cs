using OrderService.Application.Commands.UpdateOrderAddress;

namespace OrderService.Tests.UnitTests.Commands.UpdateOrderAddress
{
    [TestFixture]
    public class UpdateOrderAddressCommandValidatorTests
    {
        [Test]
        public async Task UpdateOrderAddressCommandValidatorIsValid()
        {
            var validator = new UpdateOrderAddressCommandValidator();
            var command = new UpdateOrderAddressCommand()
            {
                OrderId = Guid.NewGuid(),
                AddressLine = "AddressLine",
                AddressName = "AddressName",
                EirCode = "EirCode",
                County = "County",
            };

            var result = await validator.ValidateAsync(command);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsValid);
                Assert.That(!result.Errors.Any());
            });
        }

        [TestCase("OrderId")]
        [TestCase("AddressLine")]
        [TestCase("AddressName")]
        [TestCase("EirCode")]
        [TestCase("County")]
        public async Task UpdateOrderAddressCommandValidatorContainsErrorMessageForRequiredFields(string field)
        {
            var result = await new UpdateOrderAddressCommandValidator().ValidateAsync(new UpdateOrderAddressCommand());

            Assert.Multiple(() =>
            {
                Assert.That(!result.IsValid);
                Assert.That(result.Errors.Any(a => a.ErrorMessage == $"{{{field}}} is required."));
            });
        }

        [TestCase("AddressLine", 200)]
        [TestCase("AddressName", 50)]
        [TestCase("EirCode", 7)]
        [TestCase("County", 20)]
        public async Task UpdateOrderAddressCommandValidatorContainsErrorMessageForExceedCharacters(string field, int maxLength)
        {
            var validator = new UpdateOrderAddressCommandValidator();
            var command = new UpdateOrderAddressCommand()
            {
                AddressLine = "AddressLineAddressLineAddressLineAddressLineAddressLineAddressLineAddressLineAddressLineAddressLineAAddressLineAddressLineAddressLineAddressLineAddressLineAddressLineAddressLineAddressLineAddressLineAddressLineAddressLineAddressLineAddressLineAddressLineddressLineAddressLineAddressLineAddressLineAddressLine",
                AddressName = "AddressNameAddressNameAddressNameAddressNameAddressNameAddressNameAddressNameAddressNameAddressNameAddressNameAddressNameAddressNameAddressNameAddressName",
                EirCode = "EirCodeEirCode",
                County = "CountyCountyCountyCountyCountyCountyCountyCountyCountyCountyCountyCountyCountyCountyCountyCountyCountyCountyCountyCountyCountyCountyCountyCounty",
            };

            var result = await validator.ValidateAsync(command);

            Assert.Multiple(() =>
            {
                Assert.That(!result.IsValid);
                Assert.That(result.Errors.Any(a => a.ErrorMessage == $"{{{field}}} must not exceed {maxLength} characters."));
            });
        }
    }
}
