using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
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
