using FluentValidation;

namespace OrderService.Application.Queries.GetOrderById
{
    public class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
    {
        public GetOrderByIdQueryValidator()
        {
            RuleFor(p => p.OrderId).NotEmpty().WithMessage("{OrderId} is required.").NotNull();
        }
    }
}
