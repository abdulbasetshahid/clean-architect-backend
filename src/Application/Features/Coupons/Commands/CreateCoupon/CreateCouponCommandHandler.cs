using EShop.Application.Contracts.Persistence;
using EShop.Application.Exceptions;
using EShop.Domain.Entities;
using MediatR;

namespace EShop.Application.Features.Coupons.Commands.CreateCoupon;

public class CreateCouponCommandHandler : IRequestHandler<CreateCouponCommand, Guid>
{
    private readonly ICouponRepository _couponRepository;

    public CreateCouponCommandHandler(ICouponRepository couponRepository)
    {
        _couponRepository = couponRepository;
    }

    public async Task<Guid> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
    {
        var code = CouponConstraints.NormalizeCode(request.Code);
        if (await _couponRepository.CodeExistsAsync(code, excludeId: null, cancellationToken))
            throw new BadRequestException($"Coupon code '{code}' already exists.");

        var coupon = new Coupon
        {
            Id = Guid.NewGuid(),
            Code = code,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            DiscountType = request.DiscountType,
            Value = request.Value,
            MaxDiscountAmount = request.MaxDiscountAmount,
            MinOrderAmount = request.MinOrderAmount,
            StartsAt = request.StartsAt,
            ExpiresAt = request.ExpiresAt,
            UsageLimit = request.UsageLimit,
            UsedCount = 0,
            IsActive = request.IsActive
        };

        await _couponRepository.AddAsync(coupon);
        return coupon.Id;
    }
}
