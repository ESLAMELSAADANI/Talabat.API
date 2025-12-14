using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications;
using Talabat.Core.Specifications.Product_Specs;


namespace Talabat.Application.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;


        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IReadOnlyList<Product>> GetProductsWithSpecAsync(ISpecification<Product> spec)
            => await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);


        public async Task<Product?> GetProductWithSpecAsync(int productId)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(productId);

            var product = await _unitOfWork.Repository<Product>().GetByIdWithSpecAsync(spec);

            return product;
        }
        public async Task<IReadOnlyList<ProductBrand>> GetBrandsAsync()
            => await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
        public async Task<ProductBrand?> GetBrandAsync(int brandId)
            => await _unitOfWork.Repository<ProductBrand>().GetByIdAsync(brandId);

        public async Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync()
            => await _unitOfWork.Repository<ProductCategory>().GetAllAsync();

        public async Task<ProductCategory?> GetCategoryAsync(int categoryId)
            => await _unitOfWork.Repository<ProductCategory>().GetByIdAsync(categoryId);
        public async Task<int> GetCountAsync(ISpecification<Product> spec)
        {
            var countSpec = new ProductsWithFilterationForCountSpecification(spec.Criteria);

            var count = await _unitOfWork.Repository<Product>().GetCountAsync(countSpec);

            return count;
        }

    }
}
