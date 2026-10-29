using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Domain.Entities;
using MediatR;

namespace EShop.Application.Features.Coupons.Commands.UpdateCoupon;

public class UpdateCouponCommandHandler : IRequestHandler<UpdateCouponCommand, Unit>
{
    private readonly ICouponRepository _couponRepository;

    public UpdateCouponCommandHandler(ICouponRepository couponRepository)
    {
        _couponRepository = couponRepository;
    }

    public async Task<Unit> Handle(UpdateCouponCommand request, CancellationToken cancellationToken)
    {
        var coupon = await _couponRepository.GetByIdAsync(request.Id);
        if (coupon is null)
            throw new NotFoundException(nameof(Coupon), request.Id);

        var code = CouponConstraints.NormalizeCode(request.Code);
        if (await _couponRepository.CodeExistsAsync(code, request.Id, cancellationToken))
            throw new BadRequestException($"Coupon code '{code}' already exists.");

        coupon.Code = code;
        coupon.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        coupon.DiscountType = request.DiscountType;
        coupon.Value = request.Value;
        coupon.MaxDiscountAmount = request.MaxDiscountAmount;
        coupon.MinOrderAmount = request.MinOrderAmount;
        coupon.StartsAt = request.StartsAt;
        coupon.ExpiresAt = request.ExpiresAt;
        coupon.UsageLimit = request.UsageLimit;
        coupon.IsActive = request.IsActive;

        await _couponRepository.UpdateAsync(coupon);
        return Unit.Value;
    }
}
