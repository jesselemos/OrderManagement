using FluentValidation;

namespace OrderService.Application.Commands.UpdateOrderItems
{
    public class UpdateOrderItemsCommandValidator : AbstractValidator<UpdateOrderItemsCommand>
    {
        public UpdateOrderItemsCommandValidator()
        {
            RuleFor(p => p.OrderId)
                .NotEmpty().WithMessage("{OrderId} is required.").NotNull();

            RuleFor(p => p.OrderItems)
                .NotEmpty().WithMessage("{OrderItems} is required.");
        }
    }
}
