using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        private IMediator? _mediator;

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>() ?? throw new InvalidOperationException("IMediator service is not registered.");
       
        protected ActionResult HandleResult<T>(Result<T> result)
        {
            if (result.isSuccess && result.Value is not null)
            {
                return Ok(result.Value);
            }
            if (!result.isSuccess && result.Code == 404)
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

    }
}
