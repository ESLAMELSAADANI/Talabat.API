using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.Services.Contract
{
    public interface IProductService
    {
        Task<IReadOnlyList<Product>> GetProductsWithSpecAsync(ISpecification<Product> spec);
        Task<Product?> GetProductWithSpecAsync(int productId);
        Task<IReadOnlyList<ProductBrand>> GetBrandsAsync();
        Task<ProductBrand?> GetBrandAsync(int brandId);
        Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync();
        Task<ProductCategory?> GetCategoryAsync(int categoryId);
        Task<int> GetCountAsync(ISpecification<Product> spec);
    }
}
