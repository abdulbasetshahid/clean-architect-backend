using EShop.Domain.Enums;
using FluentValidation;

namespace EShop.Application.Features.Coupons;

public abstract class CouponCommandValidator<T> : AbstractValidator<T> where T : CouponCommandBase
{
    protected CouponCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(CouponConstraints.CodeMaxLength)
            .Matches(CouponConstraints.CodePattern)
            .WithMessage("Coupon code may only contain letters, digits, '-' and '_'.");

        RuleFor(x => x.Description)
            .MaximumLength(CouponConstraints.DescriptionMaxLength);

        RuleFor(x => x.DiscountType).IsInEnum();

        RuleFor(x => x.Value).GreaterThan(0);

        RuleFor(x => x.Value)
            .LessThanOrEqualTo(100)
            .When(x => x.DiscountType == CouponDiscountType.Percentage)
            .WithMessage("Percentage coupons cannot exceed 100.");

        RuleFor(x => x.MaxDiscountAmount)
            .GreaterThan(0)
            .When(x => x.MaxDiscountAmount.HasValue);

        RuleFor(x => x.MinOrderAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinOrderAmount.HasValue);

        RuleFor(x => x.UsageLimit)
            .GreaterThan(0)
            .When(x => x.UsageLimit.HasValue);

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(x => x.StartsAt)
            .When(x => x.StartsAt.HasValue && x.ExpiresAt.HasValue)
            .WithMessage("ExpiresAt must be after StartsAt.");
    }
}
