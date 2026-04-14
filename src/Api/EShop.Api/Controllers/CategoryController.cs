

namespace EShop.Api.Controllers;


public class CategoryController : BaseApiController
{
    private readonly IMediator _mediator;
    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetCategoryListQuery();

        var categories = await _mediator.Send(query);

        return Ok(categories);
    }
}
