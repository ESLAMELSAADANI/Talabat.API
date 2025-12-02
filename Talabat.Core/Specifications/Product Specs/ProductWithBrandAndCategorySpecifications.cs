using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.Product_Specs
{
    public class ProductWithBrandAndCategorySpecifications : BaseSpecifications<Product>
    {
        //When Query Of Get All Products => There are only includes() specs not other like where() 
        //This constructor will be used for creating an object, that will be get all products - query of get all products.
        public ProductWithBrandAndCategorySpecifications(ProductSpecParams specParams)
            : base(p =>
                        (string.IsNullOrEmpty(specParams.Search) || p.Name.ToLower().Contains(specParams.Search)) &&
                        (!specParams.BrandId.HasValue || p.BrandId == specParams.BrandId.Value) &&
                        (!specParams.CategoryId.HasValue || p.CategoryId == specParams.CategoryId.Value)
            )
        {
            //AddFilters(brandId, categoryId);

            AddSort(specParams.Sort);
            AddIncludes();
            //Total Products = 18
            //PageSize = 5 Products
            //PagesNumber = 18~20 / 5 =>  4
            //4 pages => Every Page Contain 5 products, last page contain 3 products
            //pageIndex = 3 => Mean i need the page number 3 mean the third five products from 15 - 20.
            //skip 10 and take 5.
            ApplyPagination((specParams.PageIndex - 1) * specParams.PageSize, specParams.PageSize);
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
