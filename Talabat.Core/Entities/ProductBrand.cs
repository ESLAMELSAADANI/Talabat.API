using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities
{
    public class ProductBrand : BaseEntity
    {
        public string Name { get; set; }

        //But in fact i don't need from ProductBrand can access products od this brand, so don't need to write it in code.
        //But using fluent Api i need to configure the relation One-Many to tell efcore that it's not one-one relation 
        
        //public ICollection<Product> Products { get; set; } = new HashSet<Product>();//Navigational property [Many]
    }
}
