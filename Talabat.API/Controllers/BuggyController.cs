using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.Errors;
using Talabat.Infrastructure.Data;

namespace Talabat.API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly StoreContext _dbContext;

        public BuggyController(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("notfound")]//Get: api/Buggy/notfound => ShowResponse Of Not Found Response.
        [EndpointSummary("Test response of not found resource")]
        [ProducesResponseType(typeof(Ok), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public ActionResult GetNotFoundRequest()//404 Status Code.
        {
            var product = _dbContext.Products.Find(100);//There is no product with id = 100
            if (product == null)
                return NotFound(new ApiResponse(404, "Not Found"));
            return Ok(product);
        }

        [HttpGet("servererror")]//Get : api/Buggy/servererror
        [EndpointSummary("Test response of server error")]
        [ProducesResponseType(typeof(Ok), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
        public ActionResult GetServerError()//500 Status Code.
        {
            var product = _dbContext.Products.Find(100);//There is no product with id = 100
            var productToReturn = product.ToString();//This line will through an exception => null reference exception (Server Error)

            return Ok(productToReturn);
        }

        [HttpGet("badrequest")]//Get : api/Buggy/badrequest
        [EndpointSummary("Test response of bad request")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public ActionResult GetBadRequest()//400 Status Code.
        {
            return BadRequest(new ApiResponse(400, "Bad Request!"));
        }

        [HttpGet("validationerror/{id}")]// Get : api/Buggy/validationerror/{id}
        [EndpointSummary("Test response of validatio error when send invalid data or missed data")]
        [ProducesResponseType(typeof(Ok), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
        public ActionResult GetValidationError(int id)// When Consume this end point  i will provide string value not int => validation error.
        {
            //Validation error categorized as badrequest => 400 BadRequest
            return Ok();
        }
    }
}
