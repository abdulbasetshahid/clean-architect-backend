using Microsoft.AspNetCore.Authorization;

namespace EShop.Api.Controllers.Base;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BaseApiController : ControllerBase
{
}
