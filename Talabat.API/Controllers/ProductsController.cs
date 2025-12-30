using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.DTOs;
using Talabat.API.Errors;
using Talabat.API.Helpers;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.Product_Specs;
using Talabat.Application.ProductService;

namespace Talabat.API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductsController(
            IProductService productService,
            IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        //[Authorize]
        [CachedAttribute(600)]//Action Filter Attribute To Create object from this class/filter in runtime and execute the OnActionExecutionAsync() method before excute the endpoint.
        [HttpGet]//Routing Filter QAttribute => Get : /api/products
        [EndpointSummary("Get all products")]
        [ProducesResponseType(typeof(IReadOnlyList<ProductToReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Pagination<ProductToReturnDTO>>> GetProducts([FromQuery] ProductSpecParams specParams)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(specParams);
            var products = await _productService.GetProductsWithSpecAsync(spec);

            if (products.Count == 0)
                return NotFound(new ApiResponse(StatusCodes.Status404NotFound));

            var productsDTO = _mapper.Map<IReadOnlyList<ProductToReturnDTO>>(products);
            var countSpec = new ProductsWithFilterationForCountSpecification(spec.Criteria);
            var count = await _productService.GetCountAsync(countSpec);

            var result = new Pagination<ProductToReturnDTO>(specParams.PageIndex, specParams.PageSize, count, productsDTO);
            return Ok(result);
        }

        [CachedAttribute(600)]
        [HttpGet("{id}")]
        [EndpointSummary("Get product by it's id")]
        [ProducesResponseType(typeof(ProductToReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDTO>> GetProduct(int id)
        {
            var result = await _productService.GetProductWithSpecAsync(id);
            return result is null ? BadRequest(new ApiResponse(StatusCodes.Status404NotFound)) : Ok(_mapper.Map<ProductToReturnDTO>(result));
        }

        [CachedAttribute(600)]
        [HttpGet("brands")]
        [EndpointSummary("Get all products brands")]
        [ProducesResponseType(typeof(IReadOnlyList<ProductBrand>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
            => Ok(await _productService.GetBrandsAsync());

        [CachedAttribute(600)]
        [HttpGet("brands/{id}")]
        [EndpointSummary("Get product brand by it's id")]
        [ProducesResponseType(typeof(ProductBrand), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductBrand>> GetProductBrand(int id)
        {
            var result = await _productService.GetBrandAsync(id);
            return result is null ? BadRequest(new ApiResponse(StatusCodes.Status404NotFound)) : Ok(result);
        }

        [CachedAttribute(600)]
        [HttpGet("categories")]
        [EndpointSummary("Get all product categories")]
        [ProducesResponseType(typeof(IReadOnlyList<ProductCategory>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<ProductCategory>>> GetProductCategories()
            => Ok(await _productService.GetCategoriesAsync());

        [CachedAttribute(600)]
        [HttpGet("categories/{id}")]
        [EndpointSummary("Get product category by it's id")]
        [ProducesResponseType(typeof(ProductCategory), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductCategory>> GetProductCategory(int id)
        {
            var result = await _productService.GetCategoryAsync(id);
            return result is null ? BadRequest(new ApiResponse(StatusCodes.Status404NotFound)) : Ok(result);
        }

    }
}
