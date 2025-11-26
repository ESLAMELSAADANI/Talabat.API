using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Talabat.API.Errors;

namespace Talabat.API.Controllers
{
    [Route("errors/{code}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]//To tell swagger not document APIs of this controller.
    public class ErrorsController : ControllerBase
    {
        [HttpGet]
        public ActionResult Error(int code)
        {
            if (code == (int)HttpStatusCode.Unauthorized)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized));
            else if (code == (int)HttpStatusCode.NotFound)
                return NotFound(new ApiResponse((int)HttpStatusCode.NotFound));
            else
                return StatusCode(code);
        }
    }
}
