using MediatR;

namespace EShop.Application.Features.Coupons.Queries.GetCoupons;

public class GetCouponListQuery : IRequest<IReadOnlyList<CouponVm>>
{
    public bool IncludeInactive { get; set; } = true;
}
