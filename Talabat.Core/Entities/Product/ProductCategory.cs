namespace Talabat.Core.Entities
{
    public class ProductCategory : BaseEntity
    {
        public string Name { get; set; }

        //Not need from category can access products of this category, so i will configure the relationship using fluentApi.
        //public ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}
