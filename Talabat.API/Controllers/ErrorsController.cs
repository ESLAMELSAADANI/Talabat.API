using Microsoft.AspNetCore.Mvc;
using System.Net;
using Talabat.API.Errors;

namespace Talabat.API.Controllers
{
    [Route("errors/{code}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Error(int code)
        {
            // Use ApiResponse for all common codes
            var response = new ApiResponse(code);

            return new ObjectResult(response)
            {
                StatusCode = code
            };
        }
    }
}
