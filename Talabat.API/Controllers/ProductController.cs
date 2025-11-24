using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Talabat.API.DTOs;
using Talabat.API.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications;
using Talabat.Core.Specifications.Product_Specs;

namespace Talabat.API.Controllers
{
    //[Route("api/[controller]")] 

    //[ApiController]
    public class ProductController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IMapper _mapper;

        public ProductController(IGenericRepository<Product> productRepo, IMapper mapper)
        {
            _productRepo = productRepo;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductToReturnDTO>>> GetProducts()
        {
            //var products = await _productRepo.GetAllAsync();

            var spec = new ProductWithBrandAndCategorySpecifications();
            var products = await _productRepo.GetAllWithSpecAsync(spec);

            var productsDTO = _mapper.Map<IEnumerable<ProductToReturnDTO>>(products);

            return Ok(productsDTO);
        }
        [HttpGet("{id}")]
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
    }
}
