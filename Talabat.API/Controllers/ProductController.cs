using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.DTOs;
using Talabat.API.Errors;
using Talabat.API.Helpers;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications.Product_Specs;

namespace Talabat.API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<ProductBrand> _productBrandRepo;
        private readonly IGenericRepository<ProductCategory> _productCategoryRepo;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> productRepo, IGenericRepository<ProductBrand> productBrandRepo, IGenericRepository<ProductCategory> productCategoryRepo, IMapper mapper)
        {
            _productRepo = productRepo;
            _productBrandRepo = productBrandRepo;
            _productCategoryRepo = productCategoryRepo;
            _mapper = mapper;
        }
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize]
        [HttpGet]// Get : /api/products
        [ProducesResponseType(typeof(IReadOnlyList<ProductToReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Pagination<ProductToReturnDTO>>> GetProducts([FromQuery] ProductSpecParams specParams)
        {
            //var products = await _productRepo.GetAllAsync();

            var spec = new ProductWithBrandAndCategorySpecifications(specParams);
            var products = await _productRepo.GetAllWithSpecAsync(spec);

            if (products == null || products.Count() == 0)
                return NotFound(new ApiResponse(StatusCodes.Status404NotFound));

            var productsDTO = _mapper.Map<IReadOnlyList<ProductToReturnDTO>>(products);

            var countSpec = new ProductsWithFilterationForCountSpecification(spec.Criteria);

            var count = await _productRepo.GetCountAsync(countSpec);

            var result = new Pagination<ProductToReturnDTO>(specParams.PageIndex, specParams.PageSize, count, productsDTO);

            return Ok(result);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductToReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDTO>> GetProduct(int id)
        {
            //var product = await _productRepo.GetAsync(id);

            var spec = new ProductWithBrandAndCategorySpecifications(id);
            var product = await _productRepo.GetWithSpecAsync(spec);

            if (product == null)
                return NotFound(new ApiResponse(404, "Not Found!"));//404

            var productDTO = _mapper.Map<ProductToReturnDTO>(product);
            return Ok(productDTO);//200
        }

        [HttpGet("brand")]
        [ProducesResponseType(typeof(IReadOnlyList<ProductBrand>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ProductBrand>>> GetProductBrands()
        {
            var brands = await _productBrandRepo.GetAllAsync();

            if (brands == null || brands.Count() == 0)
                return NotFound(new ApiResponse(StatusCodes.Status404NotFound));
            return Ok(brands);
        }
        [HttpGet("brand/{id}")]
        [ProducesResponseType(typeof(ProductBrand), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductBrand>> GetProductBrand(int id)
        {
            var brand = await _productBrandRepo.GetAsync(id);

            if (brand == null)
                return NotFound(new ApiResponse(StatusCodes.Status404NotFound));
            return Ok(brand);
        }
        [HttpGet("category")]
        [ProducesResponseType(typeof(IReadOnlyList<ProductCategory>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetProductCategories()
        {
            var categories = await _productCategoryRepo.GetAllAsync();
            if (categories == null || categories.Count() == 0)
                return NotFound(new ApiResponse(StatusCodes.Status404NotFound));
            return Ok(await _productCategoryRepo.GetAllAsync());
        }
        [HttpGet("category/{id}")]
        [ProducesResponseType(typeof(ProductCategory), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductCategory>> GetProductCategory(int id)
        {
            var category = await _productCategoryRepo.GetAsync(id);
            if (category == null)
                return NotFound(new ApiResponse(StatusCodes.Status404NotFound));
            return Ok(await _productCategoryRepo.GetAsync(id));
        }
    }
}
