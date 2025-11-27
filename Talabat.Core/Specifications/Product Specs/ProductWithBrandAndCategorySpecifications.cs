using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.Product_Specs
{
    public class ProductWithBrandAndCategorySpecifications : BaseSpecifications<Product>
    {
        //When Query Of Get All Products => There are only includes() specs not other like where() 
        //This constructor will be used for creating an object, that will be get all products - query of get all products.
        public ProductWithBrandAndCategorySpecifications(string? sort, int? brandId, int? categoryId) : base(p => (!brandId.HasValue || p.BrandId == brandId.Value) && (!categoryId.HasValue || p.CategoryId == categoryId.Value))
        {
            //AddFilters(brandId, categoryId);
            AddSort(sort);
            AddIncludes();
        }


        //When Query Of Get Product By ID => There are where() spec and includes() specs
        //This constructor will be used for creating an object, that will be used to get a specific product with id - query of get product by id.
        public ProductWithBrandAndCategorySpecifications(int id) : base(p => p.Id == id)
        {
            AddIncludes();
        }
        private void AddIncludes()
        {
            Includes.Add(p => p.Brand);
            Includes.Add(p => p.Category);
        }
        private void AddSort(string? sort)
        {
            if (!string.IsNullOrEmpty(sort))
                switch (sort)
                {
                    case "PriceAsc":
                        SetOrderBy(p => p.Price);
                        break;
                    case "PriceDesc":
                        SetOrderByDesc(p => p.Price);
                        break;
                    case "NameAsc":
                        SetOrderBy(p => p.Name);
                        break;
                    case "NameDesc":
                        SetOrderByDesc(p => p.Name);
                        break;
                    default:
                        SetOrderBy(p => p.Name);
                        break;
                }
            else
                SetOrderBy(p => p.Name);//if sort = null - consumer not pass value for sort parameter when consume getAll() endpoint.
        }
        //private void AddFilters(int? brandId, int? categoryId)
        //{
        //    if (brandId != null && categoryId != null)
        //        Criteria = p => p.BrandId == brandId && p.CategoryId == categoryId;
        //    else if (brandId != null && categoryId == null)
        //        Criteria = p => p.BrandId == brandId;
        //    else if (brandId == null && categoryId != null)
        //        Criteria = p => p.CategoryId == categoryId;
        //}

    }
}
