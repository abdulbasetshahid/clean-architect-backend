using MediatR;

namespace EShop.Application.Features.Coupons.Commands.UpdateCoupon;

public class UpdateCouponCommand : CouponCommandBase, IRequest<Unit>
{
    public Guid Id { get; set; }
}
