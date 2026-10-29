using EShop.Application.Features.Coupons.Commands.CreateCoupon;
using EShop.Application.Features.Coupons.Commands.UpdateCoupon;
using EShop.Application.Features.Coupons.Queries.GetCouponById;
using EShop.Application.Features.Coupons.Queries.GetCoupons;

namespace EShop.Api.Controllers;

public class CouponController : BaseApiController
{
    private readonly IMediator _mediator;

    public CouponController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = true)
    {
        var coupons = await _mediator.Send(new GetCouponListQuery { IncludeInactive = includeInactive });
        return Ok(coupons);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var coupon = await _mediator.Send(new GetCouponByIdQuery(id));
        return Ok(coupon);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCouponCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCouponCommand command)
    {
        command.Id = id;
        await _mediator.Send(command);
        return NoContent();
    }
}
