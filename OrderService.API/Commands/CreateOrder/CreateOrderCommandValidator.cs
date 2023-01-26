using FluentValidation;

namespace OrderService.API.Commands.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(p => p.CustomerName)
                .NotEmpty().WithMessage("{CustomerName} is required.")
                .NotNull()
                .MaximumLength(150).WithMessage("{CustomerName} must not exceed 150 characters.");

            RuleFor(p => p.AddressLine)
                .NotEmpty().WithMessage("{AddressLine} is required.")
                .NotNull()
                .MaximumLength(200).WithMessage("{AddressLine} must not exceed 200 characters.");

            RuleFor(p => p.AddressName)
                .NotEmpty().WithMessage("{AddressName} is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("{AddressName} must not exceed 50 characters.");

            RuleFor(p => p.EirCode)
                .NotEmpty().WithMessage("{EirCode} is required.")
                .NotNull()
                .MaximumLength(7).WithMessage("{EirCode} must not exceed 7 characters.");

            RuleFor(p => p.County)
                .NotEmpty().WithMessage("{County} is required.")
                .NotNull()
                .MaximumLength(20).WithMessage("{County} must not exceed 20 characters.");

            RuleFor(p => p.OrderItems)
                .NotEmpty().WithMessage("{OrderItems} is required.");
        }
    }
}
