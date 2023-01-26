using OrderService.API.Commands.CreateOrder;
using OrderService.API.Models;

namespace OrderService.API.UnitTests.Commands.CreateOrder
{
    [TestFixture]
    public class CreateOrderCommandValidatorTests
    {
        [Test]
        public async Task CreateOrderCommandValidatorIsValid()
        {
            var validator = new CreateOrderCommandValidator();
            var command = new CreateOrderCommand()
            {
                CustomerName = "CustomerName",
                AddressLine = "AddressLine",
                AddressName = "AddressName",
                EirCode = "EirCode",
                County = "County",
                OrderItems = new List<CreateOrderItem>
                    {
                        new CreateOrderItem
                        {
                            ProductId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
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

        [TestCase("CustomerName")]
        [TestCase("AddressLine")]
        [TestCase("AddressName")]
        [TestCase("EirCode")]
        [TestCase("County")]
        [TestCase("OrderItems")]
        public async Task CreateOrderCommandValidatorContainsErrorMessageForRequiredFields(string field)
        {
            var result = await new CreateOrderCommandValidator().ValidateAsync(new CreateOrderCommand());

            Assert.Multiple(() =>
            {
                Assert.That(!result.IsValid);
                Assert.That(result.Errors.Any(a => a.ErrorMessage == $"{{{field}}} is required."));
            });
        }

        [TestCase("CustomerName", 150)]
        [TestCase("AddressLine", 200)]
        [TestCase("AddressName", 50)]
        [TestCase("EirCode", 7)]
        [TestCase("County", 20)]
        public async Task CreateOrderCommandValidatorContainsErrorMessageForExceedCharacters(string field, int maxLength)
        {
            var validator = new CreateOrderCommandValidator();
            var command = new CreateOrderCommand()
            {
                CustomerName = "CustomerNameCustomerNameCustomerNameCustomerNameCustomerNameCustomerNameCustomerNameCustomerNameCustomerNameCustomerNameCustomerNameCustomerNameCustomerNameCustomerNameCustomerNameCustomerName",
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
