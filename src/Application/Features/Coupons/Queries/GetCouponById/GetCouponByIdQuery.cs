using MediatR;

namespace EShop.Application.Features.Coupons.Queries.GetCouponById;

public class GetCouponByIdQuery : IRequest<CouponVm>
{
    public Guid Id { get; }

    public GetCouponByIdQuery(Guid id) => Id = id;
}
