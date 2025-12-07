using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Infrastructure.Data
{
    public static class StoreContextSeed
    {
        public async static Task SeedAsync(StoreContext _dbContext)
        {
            if (!_dbContext.ProductBrands.Any())
            {
                var brandsData = File.ReadAllText("../Talabat.Infrastructure/_Data/DataSeed/brands.json");
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);

                if (brands?.Count() > 0)
                {
                    //brands = brands.Select(b => new ProductBrand()
                    //{
                    //    Name = b.Name,
                    //    Id = 0
                    //}).ToList(); 
                    foreach (var brand in brands)
                    {
                        _dbContext.Set<ProductBrand>().Add(brand);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }

            if (!_dbContext.ProductCategories.Any())
            {
                var categoryData = File.ReadAllText("../Talabat.Infrastructure/_Data/DataSeed/categories.json");
                var categories = JsonSerializer.Deserialize<List<ProductCategory>>(categoryData);

                if (categories?.Count() > 0)
                {
                    //brands = brands.Select(b => new ProductBrand()
                    //{
                    //    Name = b.Name,
                    //    Id = 0
                    //}).ToList(); 
                    foreach (var category in categories)
                    {
                        _dbContext.Set<ProductCategory>().Add(category);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }

            if (!_dbContext.Products.Any())
            {
                var productsData = File.ReadAllText("../Talabat.Infrastructure/_Data/DataSeed/products.json");
                var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                if (products?.Count() > 0)
                {
                    //brands = brands.Select(b => new ProductBrand()
                    //{
                    //    Name = b.Name,
                    //    Id = 0
                    //}).ToList(); 
                    foreach (var product in products)
                    {
                        _dbContext.Set<Product>().Add(product);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }

            if (!_dbContext.DeliveryMethods.Any())
            {
                var deliveryMethodsData = File.ReadAllText("../Talabat.Infrastructure/_Data/DataSeed/delivery.json");
                var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodsData);

                if (deliveryMethods?.Count() > 0)
                {
                    foreach (var deliveryMethod in deliveryMethods)
                    {
                        _dbContext.Set<DeliveryMethod>().Add(deliveryMethod);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
    }
}