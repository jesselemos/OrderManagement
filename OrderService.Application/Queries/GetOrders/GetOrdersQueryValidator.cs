using FluentValidation;

namespace OrderService.Application.Queries.GetOrders
{
    public class GetOrdersQueryValidator : AbstractValidator<GetOrdersQuery>
    {
        public GetOrdersQueryValidator()
        {
            RuleFor(p => p.Take)
                .GreaterThan(0).WithMessage("{Take} is required and should be greater than zero.")
                .NotEmpty()
                .NotNull();

            RuleFor(p => p.Skip)
                .GreaterThanOrEqualTo(0).WithMessage("{Skip} is required and should be greater than or equal to zero.")
                .NotNull();
        }
    }
}
