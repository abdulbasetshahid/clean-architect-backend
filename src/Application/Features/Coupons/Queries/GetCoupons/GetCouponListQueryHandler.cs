using EShop.Application.Contracts.Persistence;
using MediatR;

namespace EShop.Application.Features.Coupons.Queries.GetCoupons;

public class GetCouponListQueryHandler : IRequestHandler<GetCouponListQuery, IReadOnlyList<CouponVm>>
{
    private readonly ICouponRepository _couponRepository;

    public GetCouponListQueryHandler(ICouponRepository couponRepository)
    {
        _couponRepository = couponRepository;
    }

    public async Task<IReadOnlyList<CouponVm>> Handle(GetCouponListQuery request, CancellationToken cancellationToken)
    {
        var coupons = await _couponRepository.ListAllAsync();

        return coupons
            .Where(c => request.IncludeInactive || c.IsActive)
            .OrderBy(c => c.Code)
            .Select(CouponVm.Map)
            .ToList();
    }
}
