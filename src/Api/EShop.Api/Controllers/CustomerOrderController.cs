using EShop.Application.Contracts;
using EShop.Application.Features.Orders.Queries.GetCustomerOrders;
using EShop.Application.Models.Authentication;

namespace EShop.Api.Controllers;

[Route("api/customer/orders")]
[ApiController]
[Authorize(Policy = CustomerAuthenticationDefaults.CustomerOnlyPolicy)]
public class CustomerOrderController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentCustomerService _currentCustomerService;

    public CustomerOrderController(IMediator mediator, ICurrentCustomerService currentCustomerService)
    {
        _mediator = mediator;
        _currentCustomerService = currentCustomerService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetList([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetCustomerOrderListQuery(RequireFirebaseUid(), pageNumber, pageSize));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var order = await _mediator.Send(new GetCustomerOrderByIdQuery(id, RequireFirebaseUid()));
        return Ok(order);
    }

    private string RequireFirebaseUid() =>
        _currentCustomerService.GetFirebaseUid()
        ?? throw new UnauthorizedException("Customer authentication is required.");
}
