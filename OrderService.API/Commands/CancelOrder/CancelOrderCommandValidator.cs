using FluentValidation;

namespace OrderService.API.Commands.CreateOrder
{
    public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderCommandValidator()
        {
            RuleFor(p => p.OrderId).NotEmpty().WithMessage("{OrderId} is required.").NotNull();
        }
    }
}
