using MediatR;

namespace EShop.Application.Features.Coupons.Commands.CreateCoupon;

public class CreateCouponCommand : CouponCommandBase, IRequest<Guid>
{
}
