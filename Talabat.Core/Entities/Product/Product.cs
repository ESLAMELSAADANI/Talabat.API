namespace Talabat.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }

        //[ForeignKey(nameof(Product.Category))] => Will Make it using data annotation
        public int CategoryId { get; set; }
        //[ForeignKey(nameof(Product.Brand))] => => Will Make it using data annotation
        public int BrandId { get; set; }

        
        public ProductCategory Category { get; set; }
        public ProductBrand Brand { get; set; }//Navigational Property [One]
    }
}
