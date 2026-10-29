using FluentValidation;

namespace EShop.Application.Features.Orders.Queries.GetCustomerOrders;

public class GetCustomerOrderListQueryValidator : AbstractValidator<GetCustomerOrderListQuery>
{
    public GetCustomerOrderListQueryValidator()
    {
        RuleFor(x => x.FirebaseUid)
            .NotEmpty();

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, OrderConstraints.MaxPageSize);
    }
}
