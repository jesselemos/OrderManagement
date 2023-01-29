using FluentValidation;

namespace OrderService.Application.Commands.UpdateOrderAddress
{
    public class UpdateOrderAddressCommandValidator : AbstractValidator<UpdateOrderAddressCommand>
    {
        public UpdateOrderAddressCommandValidator()
        {
            RuleFor(p => p.OrderId).NotEmpty().WithMessage("{OrderId} is required.").NotNull();

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
        }
    }
}
