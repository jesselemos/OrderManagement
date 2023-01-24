using FluentValidation;

namespace OrderService.API.Commands.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            //TODO::Review Validators
            //RuleFor(p => p.Order.UserName)
            //    .NotEmpty().WithMessage("{UserName} is required.")
            //    .NotNull()
            //    .MaximumLength(50).WithMessage("{UserName} must not exceed 50 characters.");

            RuleFor(p => p.Order.TotalPrice)
                .NotEmpty().WithMessage("{TotalPrice} is required.")
                .GreaterThan(0).WithMessage("{TotalPrice} should be greater than zero.");
        }
    }
}
