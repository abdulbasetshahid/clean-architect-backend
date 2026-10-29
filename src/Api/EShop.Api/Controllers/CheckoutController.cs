using EShop.Application.Contracts;
using EShop.Application.Features.Orders.Commands.PlaceCheckoutOrder;
using EShop.Application.Models.Authentication;

namespace EShop.Api.Controllers;

[Route("api/orders")]
[ApiController]
public class CheckoutController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentCustomerService _currentCustomerService;

    public CheckoutController(IMediator mediator, ICurrentCustomerService currentCustomerService)
    {
        _mediator = mediator;
        _currentCustomerService = currentCustomerService;
    }

    [HttpPost("guest")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CheckoutOrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PlaceGuestOrder([FromBody] PlaceCheckoutOrderCommand command)
    {
        command.FirebaseUid = null;
        var result = await _mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost]
    [Authorize(Policy = CustomerAuthenticationDefaults.CustomerOnlyPolicy)]
    [ProducesResponseType(typeof(CheckoutOrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PlaceCustomerOrder([FromBody] PlaceCheckoutOrderCommand command)
    {
        command.FirebaseUid = _currentCustomerService.GetFirebaseUid()
            ?? throw new UnauthorizedException("Customer authentication is required.");

        var result = await _mediator.Send(command);
        return CreatedAtAction(
            nameof(CustomerOrderController.GetById),
            "CustomerOrder",
            new { id = result.OrderId },
            result);
    }
}
