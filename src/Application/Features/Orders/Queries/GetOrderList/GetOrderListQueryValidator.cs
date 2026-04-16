using EShop.Application.Features.Orders;
using FluentValidation;

namespace EShop.Application.Features.Orders.Queries.GetOrderList;

public class GetOrderListQueryValidator : AbstractValidator<GetOrderListQuery>
{
    public GetOrderListQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, OrderConstraints.MaxPageSize);
    }
}
