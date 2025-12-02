using System.Linq.Expressions;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.Product_Specs
{
    public class ProductsWithFilterationForCountSpecification:BaseSpecifications<Product>
    {
        public ProductsWithFilterationForCountSpecification(Expression<Func<Product,bool>>? crieteria):base(crieteria)
        {
        }
    }
}
