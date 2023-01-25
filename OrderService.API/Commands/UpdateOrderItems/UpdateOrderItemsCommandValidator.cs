using FluentValidation;

namespace OrderService.API.Commands.CreateOrder
{
    public class UpdateOrderItemsCommandValidator : AbstractValidator<UpdateOrderItemsCommand>
    {
        public UpdateOrderItemsCommandValidator()
        {
            RuleFor(p => p.OrderId).NotEmpty().WithMessage("{OrderId} is required.").NotNull();

            RuleFor(p => p.OrderItems)
                .NotEmpty().WithMessage("{OrderItems} is required.");
        }
    }
}
